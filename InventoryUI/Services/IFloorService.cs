

using Shared.DTOs;

namespace InventoryUI.Services
{
    public interface IFloorService
    {
        Task<bool> AddFloorAsync(PostFloor floorDto);
        Task<List<GetFloor>> GetFloorsAsync();
        Task<List<GetRoom>> GetRoomsAsync();
        Task<bool> AddRoomAsync(PostRoom roomDto);
        Task<GetFloor?> GetFloorWithRoomsAsync(int floorId);
        Task<(bool Success, string Message)> AddAssetAsync(PostAsset assetDto);
        Task<PagedResult<GetAsset>> GetAssetsByRoomAsync(int roomId, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
        Task<bool> DeleteAssetAsync(int id);
        Task<GetAsset?> GetAssetByIdAsync(int id);
        Task<bool> UpdateAssetAsync(int id, PostAsset assetDto);
        Task<int> StartSessionAsync(CreateAuditSessionDTO dto);
        Task<ScanResultDTO> ScanBarcodeAsync(ScanBarcodeDTO dto);
        Task<AuditSummaryDTO> GetSessionSummaryAsync(int sessionId);
        Task<string> ReconcileSessionAsync(int sessionId);
        Task<ReconciliationReportDTO> GetReconciliationReportAsync(int sessionId);
        Task<PagedResult<AuditSessionListDTO>> GetAllAuditSessionsAsync(int pageNumber, int pageSize);
        Task<ScannedItemDTO> AddScanToSessionAsync(int sessionId, long barcode);
        Task FinalizeAuditSessionAsync(int sessionId);
    }
}
