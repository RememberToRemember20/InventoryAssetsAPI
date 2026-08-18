using MediatR;

namespace InventoryAssetsAPI.Queries
{
    public class FinalizeSessionCommand : IRequest<bool>
    {
        public int SessionId { get; set; }
    }
}
