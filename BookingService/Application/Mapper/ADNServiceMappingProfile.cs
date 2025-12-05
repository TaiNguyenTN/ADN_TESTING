using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.ServiceDTOs;
using AutoMapper;
using Domain.Aggregate;

namespace Application.Mapper
{
    public class ADNServiceMappingProfile : Profile
    {
        public ADNServiceMappingProfile()
        {
            CreateMap<Service, ServiceResponseDTO>()
                .ForMember(dest => dest.TestCategorieNames,
                            opt => opt.MapFrom(src => src.TestCategories
                            .Select(tc => tc.CategoryName).Distinct().ToList()))
                .ForMember(dest => dest.TestPurposeNames, 
                            opt => opt.MapFrom(src => src.ServiceTestPurposes
                            .Where(stp => stp.TestPurpose != null)
                            .Select(tp => tp.TestPurpose.TestPurposeName).Distinct().ToList()));
        }
    }
}
