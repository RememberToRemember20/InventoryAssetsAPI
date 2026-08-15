using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public enum ScanStatus
    {
        Matched = 1,     // مطابق (الأصل في مكانه الصحيح)
        Misplaced = 2,   // منقول (الأصل موجود هنا لكنه مخصص لغرفة أخرى)
        Unexpected = 3
    }
}
