using AutoMapper;
using Shared.DTOs;


namespace InventoryAssetsAPI.Models.Entity
{
    public class MapperInitilizer:Profile
    {
        public MapperInitilizer()
        {
            CreateMap<Floor, GetFloor>().ReverseMap();
            CreateMap<Floor, PostFloor>().ReverseMap();
            CreateMap<Room, GetRoom>().ReverseMap();
            CreateMap<Room, PostRoom >().ReverseMap();
            CreateMap <Asset, GetAsset>().ReverseMap(); 
            CreateMap<Asset, PostAsset>().ReverseMap();
            // 1. إنشاء الجلسة: من DTO إلى Entity
            CreateMap<CreateAuditSessionDTO, AuditSession>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // يتم تعيينه برمجياً
                .ForMember(dest => dest.IsClosed, opt => opt.Ignore())  // افتراضياً false
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.Ignore());

            // 2. ملخص الجرد: من Entity إلى DTO
            CreateMap<AuditSession, AuditSummaryDTO>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))

                // حساب الأصول المطابقة من التفاصيل
                .ForMember(dest => dest.MatchedCount, opt =>
                    opt.MapFrom(src => src.Details.Count(d => d.Status == ScanStatus.Matched)))

                // حساب الأصول المنقولة من التفاصيل
                .ForMember(dest => dest.MisplacedCount, opt =>
                    opt.MapFrom(src => src.Details.Count(d => d.Status == ScanStatus.Misplaced)))

                // حساب الأصول غير المسجلة من التفاصيل
                .ForMember(dest => dest.UnexpectedCount, opt =>
                    opt.MapFrom(src => src.Details.Count(d => d.Status == ScanStatus.Unexpected)))

                // ملاحظة: TotalExpectedAssets و MissingCount تحتاج استعلام منفصل 
                // لذلك نتجاهلها في الـ Mapper وسنحسبها في الـ Controller
                .ForMember(dest => dest.TotalExpectedAssets, opt => opt.Ignore())
                .ForMember(dest => dest.MissingCount, opt => opt.Ignore());
            CreateMap<AuditDetail, AuditDetailDTO>()
    .ForMember(dest => dest.AssetName, opt =>
        opt.MapFrom(src => src.Asset != null ? src.Asset.Name : "غير معروف"));
        }
    }
}
