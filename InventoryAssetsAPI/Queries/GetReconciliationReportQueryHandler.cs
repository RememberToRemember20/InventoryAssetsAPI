using InventoryAssetsAPI.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class GetReconciliationReportQueryHandler: IRequestHandler<GetReconciliationReportQuery, ReconciliationReportDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetReconciliationReportQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<AuditSessionListDTO>> Handle(GetAllAuditSessionsQuery request, CancellationToken cancellationToken)
        {
            // نستخدم مستودعك الرائع مع الـ delegates
            var sessions = await _unitOfWork.AuditSession.GetAll(
                orderBy: q => q.OrderByDescending(s => s.CreatedAt), // الأحدث أولاً
                include: q => q.Include(s => s.Room).ThenInclude(r => r.Floor)
            );

            return sessions.Select(s => new AuditSessionListDTO
            {
                Id = s.Id,
                Title = s.Title,
                RoomName = s.Room?.Name ?? "غير محدد",
                FloorName = s.Room?.Floor?.Name ?? "غير محدد",
                StartTime = s.CreatedAt,
                EndTime = s.CompletedAt,
                IsClosed = s.CompletedAt.HasValue // إذا كان هناك تاريخ إغلاق، الجلسة مغلقة
            }).ToList();
        }
        public async Task<ReconciliationReportDTO> Handle(GetReconciliationReportQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب بيانات الجلسة مع الغرفة باستخدام دالة Get الموجودة في الـ Repository
            var session = await _unitOfWork.AuditSession.Get(
                expression: s => s.Id == request.SessionId,
                include: q => q.Include(s => s.Room)
            );

            if (session == null)
                throw new Exception($"لم يتم العثور على جلسة جرد بالرقم {request.SessionId}");

            // 2. جلب جميع عمليات المسح التي تمت في هذه الجلسة
            // لاحظ كيف نستخدم ThenInclude المتسلسلة داخل متغير include الأول
            var scannedDetails = await _unitOfWork.AuditDetail.GetAll(
                expression: d => d.AuditSessionId == request.SessionId,
                include: q => q.Include(d => d.Asset)
                               .ThenInclude(a => a.Room)
                               .ThenInclude(r => r.Floor)
            );

            // 3. جلب الأصول "المتوقعة" في هذه الغرفة
            var expectedAssets = await _unitOfWork.Assets.GetAll(
                expression: a => a.RoomId == session.RoomId,
                include: q => q.Include(a => a.Room)
                               .ThenInclude(r => r.Floor)
            );

            // 4. تهيئة التقرير النهائي
            var report = new ReconciliationReportDTO
            {
                SessionId = session.Id,
                SessionTitle = session.Title,
               // ClosedAt = session. ?? DateTime.Now,
                Items = new List<ReconciliationItemDTO>()
            };

            // 5. استخراج "المطابق" و "المنقول" أو "غير المسجل"
            foreach (var detail in scannedDetails)
            {
                // ------------- التعديل هنا -------------
                // التحقق من أن الأصل موجود فعلاً في النظام
                if (detail.Asset == null)
                {
                    report.Items.Add(new ReconciliationItemDTO
                    {
                        Barcode =0, // أو يمكنك استخدام detail.Barcode إذا كان جدول AuditDetail يحتوي على حقل الباركود الممسوح
                        AssetNumber = 0,
                        AssetName = "أصل غير مسجل في النظام",
                        FloorName = "غير محدد",
                        RoomName = "غير محدد",
                        Status = ScanStatus.Misplaced // نعامله معاملة الأصل الغريب عن الغرفة
                    });

                    continue; // تخطي الأسطر القادمة والانتقال للسطر التالي في الحلقة
                }
                // ----------------------------------------

                bool isMatched = detail.Asset.RoomId == session.RoomId;

                report.Items.Add(new ReconciliationItemDTO
                {
                    Barcode = detail.Asset.BarCode,
                    AssetNumber = detail.Asset.Id,
                    AssetName = detail.Asset.Name,
                    FloorName = detail.Asset.Room?.Floor?.Name ?? "غير محدد",
                    RoomName = detail.Asset.Room?.Name ?? "غير محدد",
                    Status = isMatched ? ScanStatus.Matched : ScanStatus.Misplaced
                });
            }

            // 6. استخراج الأصول "المفقودة"
            // ------------- التعديل هنا -------------
            // يجب أن نستثني الـ Assets التي كانت Null حتى لا يحدث خطأ هنا أيضاً
            var scannedAssetIds = scannedDetails
                .Where(d => d.Asset != null)
                .Select(d => d.AssetId)
                .ToHashSet();

            var missingAssets = expectedAssets.Where(ea => !scannedAssetIds.Contains(ea.Id));
            // ----------------------------------------
            foreach (var missing in missingAssets)
            {
                report.Items.Add(new ReconciliationItemDTO
                {
                    Barcode = missing.BarCode,
                    AssetNumber = missing.Id,
                    AssetName = missing.Name,
                   // Specifications = missing.Specifications ?? "",
                    FloorName = missing.Room?.Floor?.Name ?? "غير محدد",
                    RoomName = missing.Room?.Name ?? "غير محدد",
                    Status = ScanStatus.Unexpected
                });
            }

            // 7. حساب الإحصائيات
            report.TotalMatched = report.Items.Count(i => i.Status == ScanStatus.Matched);
            report.TotalMisplaced = report.Items.Count(i => i.Status == ScanStatus.Misplaced);
            report.TotalMissing = report.Items.Count(i => i.Status == ScanStatus.Unexpected);

            return report;
        }
    }
}
