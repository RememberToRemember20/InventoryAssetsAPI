namespace Shared.DTOs
{
    public class PostFloor
    {
        public string Name { get; set; }=string.Empty;
    }
    public class GetFloor:PostFloor
    {
        public int Id { get; set; }
        public ICollection<GetRoom> Rooms { get; set; }
    }
}
