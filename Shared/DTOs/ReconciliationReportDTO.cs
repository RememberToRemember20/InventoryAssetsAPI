using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class ReconciliationReportDTO
    {
        public int SessionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public DateTime ClosedAt { get; set; }

        // إحصائيات سريعة للوحة العلوية
        public int TotalMatched { get; set; }
        public int TotalMisplaced { get; set; }
        public int TotalMissing { get; set; }

        // قائمة الأصول
        public List<ReconciliationItemDTO> Items { get; set; } = new();
    }

}
