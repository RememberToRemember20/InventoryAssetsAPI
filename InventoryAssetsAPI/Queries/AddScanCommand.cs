using MediatR;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class AddScanCommand : IRequest<ScannedItemDTO>
    {
        public int SessionId { get; set; }
        public long Barcode { get; set; } 
    }
}
