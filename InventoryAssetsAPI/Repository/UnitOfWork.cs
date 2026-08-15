using InventoryAssetsAPI.DataAccess;
using InventoryAssetsAPI.IRepository;
using InventoryAssetsAPI.Models;

namespace InventoryAssetsAPI.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        public readonly InventoryDbContext _context;
        private IGenericRepository<Floor> _floor;
        private IGenericRepository<Room> _room;
        private IGenericRepository<Asset> _asset;
        private IGenericRepository<AuditSession> _auditSession;
        private IGenericRepository<AuditDetail> _auditDetail;
        public UnitOfWork(InventoryDbContext context)   { _context = context; }
        public IGenericRepository<Floor> Floors => _floor ??= new GenericRepository<Floor>(_context);
        public IGenericRepository<Room> Rooms => _room ??= new GenericRepository<Room>(_context);
        public IGenericRepository<Asset>Assets=>_asset ??= new GenericRepository<Asset>(_context);
        public IGenericRepository<AuditSession> AuditSession => _auditSession ??=new GenericRepository<AuditSession>(_context);
        public IGenericRepository<AuditDetail> AuditDetail => _auditDetail ??=new GenericRepository<AuditDetail>(_context);
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
