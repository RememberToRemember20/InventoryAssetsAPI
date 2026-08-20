using InventoryAssetsAPI.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class GetAllAuditSessionsQueryHandler : IRequestHandler<GetAllAuditSessionsQuery, PagedResult<AuditSessionListDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAuditSessionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<AuditSessionListDTO>> Handle(GetAllAuditSessionsQuery request, CancellationToken cancellationToken)
        {
            // نستخدم مستودعك الرائع مع الـ delegates
            var pagedSessions = await _unitOfWork.AuditSession.GetPagingAll(
             include: q => q.Include(s => s.Room).ThenInclude(r => r.Floor),
             orderBy: q => q.OrderByDescending(s => s.CreatedAt),
             request: request.RequestParams
         );

            // تحويل العناصر إلى DTOs
            var dtoList = pagedSessions.Items.Select(s => new AuditSessionListDTO
            {
                Id = s.Id,
                Title = s.Title,
                RoomName = s.Room?.Name ?? "غير محدد",
                FloorName = s.Room?.Floor?.Name ?? "غير محدد",
                StartTime = s.CreatedAt,
                EndTime = s.CompletedAt,
                IsClosed = s.CompletedAt.HasValue
            }).ToList();

            // إرجاع النتيجة مغلّفة مع الـ MetaData
            // (ملاحظة: استبدل خصائص pagedSessions بأسماء الخصائص الموجودة في مكتبة IPagedList التي تستخدمها)
            return new PagedResult<AuditSessionListDTO>
            {
                Items = dtoList,
                MetaData = pagedSessions.MetaData
            };
        }
    }
}
