using AutoMapper;
using LondonEstate.ViewModels;

namespace LondonEstate.AutoMapper
{

    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            // Example mapping
            CreateMap<AgreementViewModel, Models.Agreement>();
            CreateMap<InvoiceViewModel, Models.Invoice>();

            // Add more mappings here
        }
    }
}
