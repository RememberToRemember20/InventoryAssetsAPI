using Shared.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAssetsAPI.Models
{
    public class AuditReportItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(AuditSession))]
        public int AuditSessionId { get; set; }
        public AuditSession AuditSession { get; set; }

        public int AssetId { get; set; }
        //public Asset Asset { get; set; }
        public long? Barcode { get; set; }
        public string AssetName { get; set; }

        // نحفظ اسم الغرفة واسم الطابق كنص صريح حتى لا يتأثرا بأي تعديل مستقبلي
        public string RoomNameAtAudit { get; set; }
        public string FloorNameAtAudit { get; set; }

        public ScanStatus Status { get; set; } // مطابق، منقول، مفقود
    }
}
