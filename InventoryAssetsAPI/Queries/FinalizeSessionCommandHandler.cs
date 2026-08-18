using InventoryAssetsAPI.IRepository;
using InventoryAssetsAPI.Models;
using MediatR;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class FinalizeSessionCommandHandler : IRequestHandler<FinalizeSessionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinalizeSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(FinalizeSessionCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب الجلسة
            var session = await _unitOfWork.AuditSession.Get(q=>q.Id==request.SessionId);
            if (session == null) throw new KeyNotFoundException("الجلسة غير موجودة.");
            if (session.IsClosed) return true; // مغلقة مسبقاً

            // 2. هندسة ذكية: جرد المفقودات تلقائياً عند الإغلاق
            // أ. جلب الأصول المفترض وجودها في غرفة الجلسة
            var assetsInRoom = await _unitOfWork.Assets.GetAll(a => a.RoomId == session.RoomId);

            // ب. جلب معرفات الأصول التي تم جردها فعلياً في هذه الجلسة
            var scannedAssetIds = await _unitOfWork.AuditDetail
                .GetAll(d => d.AuditSessionId == request.SessionId);
            var scannedIdsList = scannedAssetIds.Select(d => d.AssetId).ToList();

            // ج. استخراج الأصول المفقودة (الموجودة بالغرفة ولكن لم تُجرد)
            var missingAssets = assetsInRoom.Where(a => !scannedIdsList.Contains(a.Id)).ToList();

            // د. إضافة الأصول المفقودة لقائمة الجرد كـ "Missing"
            foreach (var asset in missingAssets)
            {
                await _unitOfWork.AuditDetail.Insert(new AuditDetail
                {
                    AuditSessionId = session.Id,
                    AssetId = asset.Id,
                    Status = ScanStatus.Unexpected,
                    ScannedAt = DateTime.Now
                });
            }

            // 3. إغلاق الجلسة
            session.IsClosed = true;
            session.CompletedAt = DateTime.Now;
            _unitOfWork.AuditSession.Update(session);

            // 4. حفظ التغييرات دفعة واحدة
            await _unitOfWork.Save();

            return true;
        }
    }
}
