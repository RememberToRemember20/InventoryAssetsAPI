using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class StartSessionResponseDTO
    {
        public int SessionId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
