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
        }
    }
}
