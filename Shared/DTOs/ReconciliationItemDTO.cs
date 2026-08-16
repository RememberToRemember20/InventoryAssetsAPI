using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class ReconciliationItemDTO
    {
        public long? Barcode { get; set; } 
        public int AssetNumber { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string Specifications { get; set; } = string.Empty;

        // الموقع (سواء كان موقعه الأصلي أو الغرفة التي نُقل إليها)
        public string FloorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        public ScanStatus Status { get; set; }
    }
}
