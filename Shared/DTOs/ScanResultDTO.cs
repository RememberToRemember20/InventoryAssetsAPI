using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class ScanResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ScanStatus Status { get; set; }
        public string? AssetName { get; set; }
        public long BarCode { get; set; }
        public string? ExpectedRoomName { get; set; }
        public DateTime ScannedAt { get; set; }
    }
}
