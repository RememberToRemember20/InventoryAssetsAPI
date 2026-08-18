using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.DTOs
{
    public class ScannedItemDTO
    {
        public int DetailId { get; set; }

        // بيانات الأصل الأساسية
        public long? Barcode { get; set; } = 0;
        public int AssetNumber { get; set; } 
        public string AssetName { get; set; } = string.Empty;
        public string Specifications { get; set; } = string.Empty;

        // معلومات الموقع (تفيد جداً في حالة Status = Misplaced لمعرفة أين مكانه الأصلي)
        public string FloorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        // حالة الجرد (مطابق، منقول، غريب، مفقود)
        public ScanStatus Status { get; set; }

        // وقت المسح (تستخدم في الواجهة لترتيب العناصر بحيث يظهر الأحدث في الأعلى)
        public DateTime ScannedAt { get; set; }

        // --- خصائص خاصة بالواجهة فقط (UI-Only Properties) ---

        // هذه الخاصية لن يتم إرسالها من الـ API، بل نستخدمها في Blazor فقط
        // لتلوين السطر باللون الأخضر لحظة الإضافة (Optimistic UI)
        [JsonIgnore]
        public bool IsJustAdded { get; set; }
    }
}
