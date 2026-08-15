using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class ScanBarcodeDTO
    {
        public int AuditSessionId { get; set; }
        public long BarCode { get; set; }
    }
}
