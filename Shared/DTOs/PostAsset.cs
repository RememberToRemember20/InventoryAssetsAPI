using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.DTOs
{
    public  class PostAsset
    {
        public int BarCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        public int RoomId { get; set; }
       
    }
    public class GetAsset
    {
        public int Id { get; set; }
        public int BarCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        public int RoomId { get; set; }
        public GetRoom Room { get; set; }
    }
}
