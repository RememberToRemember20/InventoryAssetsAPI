using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public  class PostRoom
    {
        public string Name { get; set; }
        public int FloorId { get; set; }
    }
    public class GetRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FloorId { get; set; }
        public PostFloor Floor { get; set; }
    }
}
