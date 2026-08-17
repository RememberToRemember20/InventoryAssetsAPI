using InventoryAssetsAPI.Models;

namespace InventoryAssetsAPI.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
        public IGenericRepository<Floor> Floors { get; }
        public IGenericRepository<Room> Rooms { get; }
        public IGenericRepository<Asset> Assets { get; }
        public IGenericRepository<AuditSession> AuditSession { get; }
        public IGenericRepository<AuditDetail> AuditDetail { get; }
        public IGenericRepository<AuditReportItem> AuditReportItems { get; }
        Task Save();
    }
}
