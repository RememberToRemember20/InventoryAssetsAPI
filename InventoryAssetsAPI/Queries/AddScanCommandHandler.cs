using AutoMapper;
using InventoryAssetsAPI.IRepository;
using InventoryAssetsAPI.Migrations;
using InventoryAssetsAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class AddScanCommandHandler : IRequestHandler<AddScanCommand, ScannedItemDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddScanCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ScannedItemDTO> Handle(AddScanCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الجلسة للتأكد من وجودها وحالتها
            var session = await _unitOfWork.AuditSession.Get(q=>q.Id == request.SessionId);

            if (session == null)
                throw new KeyNotFoundException("الجلسة المحددة غير موجودة.");

            if (session.IsClosed)
                throw new InvalidOperationException("لا يمكن إضافة أصول لجلسة مغلقة.");

            // تحويل الباركود القادم من الواجهة (string) إلى long 
            // (إذا كان الباركود في الـ Command من نوع long أساساً، يمكنك إزالة هذا السطر واستخدام request.Barcode مباشرة)
            long scannedBarcodeValue = request.Barcode;
            var isAlreadyScanned = await _unitOfWork.AuditDetail.Get(
                d => d.AuditSessionId == session.Id && d.ScannedBarCode == scannedBarcodeValue);

            if (isAlreadyScanned != null)
            {
                throw new InvalidOperationException("هذا الأصل تم جرده مسبقاً في هذه الجلسة!");
            }
            // 2. البحث عن الأصل في قاعدة البيانات باستخدام الباركود
            // نستخدم Include لجلب بيانات الغرفة والطابق لكي يستطيع AutoMapper قراءتها
            var asset = await _unitOfWork.Assets.Get(
                a => a.BarCode == scannedBarcodeValue,
                include:q=>q.Include(r=>r.Room).ThenInclude(f=>f.Floor)
            );

            // 3. تحديد حالة الجرد (Business Logic)
            ScanStatus status;
            int? expectedRoomId = asset?.RoomId; // سيكون null إذا كان الأصل غريباً

            if (asset == null)
            {
                // الأصل غير موجود نهائياً في قاعدة البيانات (غريب)
                status = ScanStatus.Unexpected;
            }
            else if (asset.RoomId == session.RoomId)
            {
                // الأصل موجود في نفس الغرفة المخصصة لهذه الجلسة
                status = ScanStatus.Matched;
            }
            else
            {
                // الأصل موجود في قاعدة البيانات لكنه مسجل في غرفة أخرى (منقول بالخطأ)
                status = ScanStatus.Misplaced;
            }

            // 4. إنشاء سجل الجرد الجديد (AuditDetail) بالهيكلة الجديدة والممتازة الخاصة بك
            var auditDetail = new AuditDetail
            {
                AuditSessionId = session.Id,
                ScannedBarCode = scannedBarcodeValue,
                AssetId = asset?.Id,
                ScannedRoomId = session.RoomId,
                ExpectedRoomId = expectedRoomId,
                Status = status,
                ScannedAt = DateTime.UtcNow
            };

            // نربط كائن Asset مباشرة لتسهيل عمل الـ AutoMapper لاحقاً (خطوة اختيارية لكنها مفيدة جداً)
            // var result = _mapper.Map<AuditDetail>(auditDetail);
          
            // 5. حفظ البيانات في قاعدة البيانات
            await  _unitOfWork.AuditDetail.Insert(auditDetail);
            await _unitOfWork.Save();
            auditDetail.Asset = asset;
            // 6. التحويل إلى DTO للإعادة للواجهة (AutoMapper سيتكفل بكل الحقول بناءً على الـ Profile)
            var resultDto = _mapper.Map<ScannedItemDTO>(auditDetail);

            // إعادة الكائن الصغير للواجهة الأمامية ليعرضه فوراً في الجدول
            return resultDto;
        }
    }
}
