using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class AuditDetailDTO
    {
        public int Id { get; set; }
        public long ScannedBarCode { get; set; }
        public int? AssetId { get; set; }
        public string? AssetName { get; set; } // اسم الأصل إن وجد
        public ScanStatus Status { get; set; }
        public DateTime ScannedAt { get; set; }
    }
}
