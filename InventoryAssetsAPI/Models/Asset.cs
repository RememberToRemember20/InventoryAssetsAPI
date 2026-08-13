using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAssetsAPI.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }
        public int? BarCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
