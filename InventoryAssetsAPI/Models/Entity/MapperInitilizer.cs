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
            CreateMap<AuditDetail, AuditDetailDTO>();
            CreateMap<AuditDetail, ScannedItemDTO>()
                // ربط المعرفات الأساسية
                .ForMember(dest => dest.DetailId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.ScannedBarCode))

                // جلب بيانات الأصل مع معالجة حالة الأصل الغريب (Asset == null)
                // بما أن AssetNumber أصبح int، نضع 0 في حال لم يكن هناك أصل
                .ForMember(dest => dest.AssetNumber, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Id : 0))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Name : "أصل غير مسجل (غريب)"))
                .ForMember(dest => dest.Specifications, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Description : ""))

                // جلب مكان الأصل المفترض (المكان الأصلي للأصل) لمعرفة من أين تم نقله إذا كان Misplaced
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src =>
                    src.Asset != null && src.Asset.Room != null ? src.Asset.Room.Name : "غير معروف"))
                .ForMember(dest => dest.FloorName, opt => opt.MapFrom(src =>
                    src.Asset != null && src.Asset.Room != null && src.Asset.Room.Floor != null ? src.Asset.Room.Floor.Name : "غير معروف"))

                // الحالة ووقت المسح
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ScannedAt, opt => opt.MapFrom(src => src.ScannedAt))

                // تجاهل الخصائص الخاصة بالواجهة فقط
                .ForMember(dest => dest.IsJustAdded, opt => opt.Ignore());
        }
        
    }
}
