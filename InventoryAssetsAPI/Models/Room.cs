using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAssetsAPI.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(Floor))]
        public int FloorId { get; set; }
        public Floor Floor { get; set; }
        public ICollection<Asset> Assets{ get; set; }
    }
}
