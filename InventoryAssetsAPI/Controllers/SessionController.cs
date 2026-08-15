using AutoMapper;
using InventoryAssetsAPI.IRepository;
using InventoryAssetsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        [HttpPost("StartSession")]
        public async Task<IActionResult> StartSession([FromBody] CreateAuditSessionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var session = _mapper.Map<AuditSession>(dto);
                session.CreatedAt = DateTime.UtcNow;
                session.IsClosed = false;

                await _unitOfWork.AuditSession.Insert(session);
                await _unitOfWork.Save();

                return Ok(new { SessionId = session.Id, Message = "تم بدء جلسة الجرد بنجاح." });
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "حدث خطأ أثناء إنشاء جلسة جرد جديدة.");
                return StatusCode(500, "حدث خطأ داخلي في الخادم.");
            }
        }
        [HttpPost("ScanBarcode")]
        public async Task<ActionResult<ScanResultDTO>> ScanBarcode([FromBody] ScanBarcodeDTO dto)
        {
            try
            {
                // 1. جلب الجلسة للتأكد من صلاحيتها
                var session = await _unitOfWork.AuditSession.Get(s => s.Id == dto.AuditSessionId);

                if (session == null)
                    return NotFound("جلسة الجرد غير موجودة.");

                if (session.IsClosed)
                    return BadRequest("لا يمكن مسح أصول جديدة، جلسة الجرد مغلقة.");

                // 2. البحث عن الأصل وجلب بيانات الغرفة المرتبطة به باستخدام دالة الـ Include في الـ Repository
                var asset = await _unitOfWork.Assets.Get(
                    expression: a => a.BarCode == dto.BarCode,
                    include: query => query.Include(a => a.Room)
                );

                ScanStatus status;
                string message;
                int? expectedRoomId = null;
                string? expectedRoomName = null;

                if (asset == null)
                {
                    status = ScanStatus.Unexpected;
                    message = "تنبيه: الباركود غير مسجل في النظام نهائياً!";
                }
                else
                {
                    expectedRoomId = asset.RoomId;
                    expectedRoomName = asset.Room?.Name ?? $"غرفة #{asset.RoomId}";

                    if (asset.RoomId == session.RoomId)
                    {
                        status = ScanStatus.Matched;
                        message = $"مطابق: {asset.Name}";
                    }
                    else
                    {
                        status = ScanStatus.Misplaced;
                        message = $"منقول: هذا الأصل مسجل حالياً في ({expectedRoomName})!";
                    }
                }

                // 3. بناء تفاصيل المسح (AuditDetail)
                var detail = new AuditDetail
                {
                    AuditSessionId = session.Id,
                    ScannedBarCode = dto.BarCode,
                    AssetId = asset?.Id,
                    ScannedRoomId = session.RoomId,
                    ExpectedRoomId = expectedRoomId,
                    Status = status,
                    ScannedAt = DateTime.UtcNow
                };

                await _unitOfWork.AuditDetail.Insert(detail);
                await _unitOfWork.Save();

                // 4. إرجاع النتيجة للواجهة
                return Ok(new ScanResultDTO
                {
                    Success = true,
                    Message = message,
                    Status = status,
                    BarCode = dto.BarCode,
                    AssetName = asset?.Name,
                    ExpectedRoomName = expectedRoomName,
                    ScannedAt = detail.ScannedAt
                });
            }
            catch (Exception ex)
            {
         //       _logger.LogError(ex, $"خطأ أثناء مسح الباركود {dto.BarCode} للجلسة {dto.AuditSessionId}");
                return StatusCode(500, "حدث خطأ أثناء معالجة الباركود.");
            }
        }
        [HttpGet("SessionSummary/{sessionId}")]
        public async Task<ActionResult<AuditSummaryDTO>> GetSessionSummary(int sessionId)
        {
            try
            {
                // 1. جلب الجلسة مع جميع التفاصيل المرتبطة بها (ومع بيانات الأصل داخل كل تفصيل)
                var session = await _unitOfWork.AuditSession.Get(
                    expression: s => s.Id == sessionId,
                    include: query => query.Include(s => s.Details).ThenInclude(d => d.Asset)
                );

                if (session == null)
                    return NotFound("جلسة الجرد غير موجودة.");

                // 2. استخدام AutoMapper لتحويل الـ Entity إلى DTO شامل
                // (هذا سيحسب MatchedCount, MisplacedCount, إلخ، كما برمجناه في ملف الـ Profile)
                var summary = _mapper.Map<AuditSummaryDTO>(session);

                // 3. حساب إجمالي الأصول المفترض وجودها في الغرفة
                // نستخدم GetAll مع فلتر الغرفة المستهدفة
                var expectedAssets = await _unitOfWork.Assets.GetAll(a => a.RoomId == session.RoomId);
                summary.TotalExpectedAssets = expectedAssets.Count;

                // 4. حساب الأصول المفقودة (المفترض وجودها ناقص ما وجدناه وطابقناه + ما تم نقله إليها بالخطأ)
                summary.MissingCount = summary.TotalExpectedAssets - (summary.MatchedCount + summary.MisplacedCount);

                return Ok(summary);
            }
            catch (Exception ex)
            {
           //     _logger.LogError(ex, $"خطأ أثناء جلب ملخص الجلسة رقم {sessionId}");
                return StatusCode(500, "حدث خطأ داخلي أثناء جلب البيانات.");
            }
        }
        [HttpPost("ReconcileSession/{sessionId}")]
        public async Task<IActionResult> ReconcileSession(int sessionId)
        {
            try
            {
                // 1. جلب الجلسة والتفاصيل
                var session = await _unitOfWork.AuditSession.Get(
                    expression: s => s.Id == sessionId,
                    include: query => query.Include(s => s.Details)
                );

                if (session == null)
                    return NotFound("جلسة الجرد غير موجودة.");

                if (session.IsClosed)
                    return BadRequest("هذه الجلسة معتمدة ومغلقة مسبقاً.");

                // 2. فلترة الأصول المنقولة (Misplaced) لتحديث أماكنها
                var misplacedDetails = session.Details
                    .Where(d => d.Status == ScanStatus.Misplaced && d.AssetId.HasValue)
                    .ToList();

                foreach (var detail in misplacedDetails)
                {
                    // جلب الأصل من قاعدة البيانات
                    var assetToMove = await _unitOfWork.Assets.Get(a => a.Id == detail.AssetId);
                    if (assetToMove != null)
                    {
                        // تحديث موقع الأصل ليكون في هذه الغرفة الجديدة
                        assetToMove.RoomId = session.RoomId;
                        _unitOfWork.Assets.Update(assetToMove); // تحديث الحالة إلى Modified
                    }
                }

                // 3. إغلاق الجلسة
                session.IsClosed = true;
                session.CompletedAt = DateTime.UtcNow;

                _unitOfWork.AuditSession.Update(session); // تحديث حالة الجلسة

                // 4. حفظ التغييرات دفعة واحدة (Transaction-like behavior)
                await _unitOfWork.Save();

                return Ok(new { Message = "تم اعتماد الجرد وتحديث مواقع الأصول بنجاح." });
            }
            catch (Exception ex)
            {
             //   _logger.LogError(ex, $"خطأ أثناء تسوية واعتماد الجلسة رقم {sessionId}");
                return StatusCode(500, "حدث خطأ داخلي أثناء اعتماد الجرد.");
            }
        }
    }
}
