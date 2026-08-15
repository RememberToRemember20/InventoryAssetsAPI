using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class CreateAuditSessionDTO
    {
        public string Title { get; set; } = string.Empty;
        public int RoomId { get; set; }
    }
}
