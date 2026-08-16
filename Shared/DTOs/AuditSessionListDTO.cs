using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
     public class AuditSessionListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FloorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsClosed { get; set; }
    }
}
