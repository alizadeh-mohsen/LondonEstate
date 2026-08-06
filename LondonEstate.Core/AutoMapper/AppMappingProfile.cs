using AutoMapper;
using LondonEstate.Core.Dtos;
using LondonEstate.Core.Models;

namespace LondonEstate.Core.AutoMapper
{

    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {

            CreateMap<Flat, FlatDto>().ReverseMap();
            CreateMap<Flat, BookingImportDto>().ReverseMap();
        }
    }
}
