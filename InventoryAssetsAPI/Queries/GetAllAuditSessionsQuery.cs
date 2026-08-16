using MediatR;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class GetAllAuditSessionsQuery : IRequest<List<AuditSessionListDTO>> { }
    
}
