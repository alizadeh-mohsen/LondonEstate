using AutoMapper;
using LondonEstate.Core.Dtos;
using LondonEstate.ViewModels;

namespace LondonEstate.Core.AutoMapper
{

    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<FlatDto, FlatViewModel>().ReverseMap();
        }
    }
}
