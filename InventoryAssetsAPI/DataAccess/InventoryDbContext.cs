using InventoryAssetsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAssetsAPI.DataAccess
{
    public class InventoryDbContext:DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // جعل الـ BarCode فريداً وغير قابل للتكرار
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.BarCode)
                .IsUnique();
        }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AuditSession>AuditSessions{ get; set; }
        public DbSet<AuditDetail> AuditDetails { get; set; }
        public DbSet<AuditReportItem> AuditReportItems { get; set; }
    }
}
