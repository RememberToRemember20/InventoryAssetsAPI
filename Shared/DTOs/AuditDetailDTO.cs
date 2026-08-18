using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.DTOs
{
    public class AuditDetailDTO
    {
        public int AuditSessionId { get; set; }
        
        public long ScannedBarCode { get; set; }
       
        public int? AssetId { get; set; } // قد يكون null إذا كان الباركود غير معروف
        public int ScannedRoomId { get; set; } // الغرفة التي جرى بها المسح
        public int? ExpectedRoomId { get; set; } // الغرفة المفترضة حسب النظام

        public ScanStatus Status { get; set; }
        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
    }
}
