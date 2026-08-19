using InventoryAssetsAPI.IRepository;
using InventoryAssetsAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class FinalizeSessionCommandHandler : IRequestHandler<FinalizeSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public FinalizeSessionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<bool> Handle(FinalizeSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.AuditSession.Get(
                expression: s => s.Id == request.SessionId,
                include: query => query.Include(s => s.Details)
            );

            if (session == null)
                throw new KeyNotFoundException("جلسة الجرد غير موجودة.");

            if (session.IsClosed)
                return true; // الجلسة مغلقة ومحفوظة مسبقاً

            // 2. نقل الأصول المنقولة (Misplaced) إلى الغرفة الجديدة في الداتابيز
            var misplacedDetails = session.Details
                .Where(d => d.Status == ScanStatus.Misplaced && d.AssetId.HasValue)
                .ToList();

            foreach (var detail in misplacedDetails)
            {
                var assetToMove = await _unitOfWork.Assets.Get(a => a.Id == detail.AssetId);
                if (assetToMove != null)
                {
                    assetToMove.RoomId = session.RoomId;
                    _unitOfWork.Assets.Update(assetToMove);
                }
            }

            // 3. احتساب الأصول المفقودة (Missing) وإضافتها صراحة لجدول AuditDetails في الداتابيز
            var scannedAssetIds = session.Details
                .Where(d => d.AssetId.HasValue)
                .Select(d => d.AssetId!.Value)
                .ToList();

            var missingAssets = await _unitOfWork.Assets.GetAll(
                a => a.RoomId == session.RoomId && !scannedAssetIds.Contains(a.Id)
            );

            foreach (var missingAsset in missingAssets)
            {
                var missingDetail = new AuditDetail
                {
                    AuditSessionId = session.Id,
                    AssetId = missingAsset.Id,
                    ScannedBarCode = missingAsset.BarCode ?? 0,
                    ScannedRoomId = session.RoomId,
                    ExpectedRoomId = missingAsset.RoomId,
                    Status = ScanStatus.Unexpected,
                    ScannedAt = DateTime.UtcNow
                };

                await _unitOfWork.AuditDetail.Insert(missingDetail);
            }

            // 4. توليد التقرير الحي شاملاً المفقودات الجديدة لتجميده
            var liveReport = await _mediator.Send(new GetReconciliationReportQuery(session.Id), cancellationToken);

            // 5. تجميد التقرير النهائي في جدول AuditReportItems
            var reportEntities = liveReport.Items.Select(item => new AuditReportItem
            {
                AuditSessionId = session.Id,
                AssetId = item.AssetNumber,
                Barcode = item.Barcode ?? 0,
                AssetName = item.AssetName,
                FloorNameAtAudit = item.FloorName,
                RoomNameAtAudit = item.RoomName,
                Status = item.Status
            }).ToList();

            await _unitOfWork.AuditReportItems.InsertRange(reportEntities);

            // 6. تغيير حالة الجلسة وتحديد وقت الإغلاق النهائي
            session.IsClosed = true;
            session.CompletedAt = DateTime.UtcNow;
            _unitOfWork.AuditSession.Update(session);

            // 7. حفظ جميع التعديلات والإضافات بشكل دائم في قاعدة البيانات
            await _unitOfWork.Save();

            return true;
        }
    }
}
