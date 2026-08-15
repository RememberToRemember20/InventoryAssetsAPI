using Shared.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAssetsAPI.Models
{
    public class AuditDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(AuditSession))]
        public int AuditSessionId { get; set; }
        public AuditSession AuditSession { get; set; } = default!;

        public long ScannedBarCode { get; set; }
        [ForeignKey(nameof(Asset))]
        public int? AssetId { get; set; } // قد يكون null إذا كان الباركود غير معروف
        public Asset? Asset { get; set; }
        public int ScannedRoomId { get; set; } // الغرفة التي جرى بها المسح
        public int? ExpectedRoomId { get; set; } // الغرفة المفترضة حسب النظام

        public ScanStatus Status { get; set; }
        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
    }
}
