using System.ComponentModel.DataAnnotations;

namespace InventoryAssetsAPI.Models
{
    public class AuditSession
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int RoomId { get; set; } // الغرفة المستهدفة بالجرد
        public Room Room { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public bool IsClosed { get; set; } = false; // هل تم اعتماد الجرد وإغلاقه؟

        public ICollection<AuditDetail> Details { get; set; } = new List<AuditDetail>();
    }
}
