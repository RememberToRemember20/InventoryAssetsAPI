using InventoryAssetsAPI.Models;

namespace InventoryAssetsAPI.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
        public IGenericRepository<Floor> Floors { get; }
        public IGenericRepository<Room> Rooms { get; }
        public IGenericRepository<Asset> Assets { get; }
        Task Save();
    }
}
