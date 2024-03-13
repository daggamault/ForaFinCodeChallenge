using AutoMapper;
using ForaFin.Api.Dtos;
using ForaFin.Api.Entities;

namespace ForaFin.Api.Profiles;

public class EdgarProfile : Profile
{
    public EdgarProfile()
    {
        CreateMap<EdgarCompanyDto, CompanyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Cik))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.EntityName))
            .ReverseMap();
        CreateMap<CompanyDto, CompanyEntity>()
            .ReverseMap();
    }
}