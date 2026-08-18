using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class ScanRequest
    {
        public int SessionId { get; set; }
        public long Barcode { get; set; }
    }
}
