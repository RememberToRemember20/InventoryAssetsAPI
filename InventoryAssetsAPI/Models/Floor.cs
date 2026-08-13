using System.ComponentModel.DataAnnotations;

namespace InventoryAssetsAPI.Models
{
    public class Floor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
