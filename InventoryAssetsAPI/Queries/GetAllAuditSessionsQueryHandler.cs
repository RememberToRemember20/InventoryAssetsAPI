using InventoryAssetsAPI.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Queries
{
    public class GetAllAuditSessionsQueryHandler : IRequestHandler<GetAllAuditSessionsQuery, List<AuditSessionListDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAuditSessionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AuditSessionListDTO>> Handle(GetAllAuditSessionsQuery request, CancellationToken cancellationToken)
        {
            // نستخدم مستودعك الرائع مع الـ delegates
            var sessions = await _unitOfWork.AuditSession.GetAll(include: q => q.Include(s => s.Room).ThenInclude(r => r.Floor),
                orderBy: q => q.OrderByDescending(s => s.CreatedAt) // الأحدث أولاً
                
            );

            return sessions.Select(s => new AuditSessionListDTO
            {
                Id = s.Id,
                Title = s.Title,
                RoomName = s.Room?.Name ?? "غير محدد",
                FloorName = s.Room?.Floor?.Name ?? "غير محدد",
                StartTime = s.CreatedAt,
                EndTime = s.CompletedAt,
                IsClosed = s.CompletedAt.HasValue // إذا كان هناك تاريخ إغلاق، الجلسة مغلقة
            }).ToList();
        }
    }
}
