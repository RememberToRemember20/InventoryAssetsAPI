

using Shared.DTOs;

namespace InventoryUI.Services
{
    public interface IFloorService
    {
        Task<bool> AddFloorAsync(PostFloor floorDto);
        Task<List<GetFloor>> GetFloorsAsync();
        Task<bool> AddRoomAsync(PostRoom roomDto);
        Task<GetFloor?> GetFloorWithRoomsAsync(int floorId);
        Task<(bool Success, string Message)> AddAssetAsync(PostAsset assetDto);
        Task<List<GetAsset>> GetAssetsByRoomAsync(int roomId);
        Task<bool> DeleteAssetAsync(int id);
    }
}
