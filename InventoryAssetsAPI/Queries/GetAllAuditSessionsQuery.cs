using MediatR;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class GetAllAuditSessionsQuery : IRequest<PagedResult<AuditSessionListDTO>> { public RequestParams RequestParams { get; set; } }
    
}
