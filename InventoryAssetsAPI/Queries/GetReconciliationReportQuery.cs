
using MediatR;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class GetReconciliationReportQuery: IRequest<ReconciliationReportDTO>
    {
        public int SessionId { get; set; }

        public GetReconciliationReportQuery(int sessionId)
        {
            SessionId = sessionId;
        }
    }
}
