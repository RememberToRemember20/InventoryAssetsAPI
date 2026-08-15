using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class AuditSummaryDTO
    {
        public int SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }

        public int TotalExpectedAssets { get; set; } // الأصول المفترض وجودها بالغرفة
        public int MatchedCount { get; set; }       // أصول مطابقة تم مسحها
        public int MisplacedCount { get; set; }     // أصول منقولة وُجدت هنا
        public int UnexpectedCount { get; set; }    // أصول غير مسجلة بالنظام
        public int MissingCount { get; set; }
        public List<AuditDetailDTO> ScannedDetails { get; set; } = new();
    }
}
