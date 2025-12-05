using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.TestPurposeDTOs;
using AutoMapper;
using Domain.Aggregate;

namespace Application.Mapper
{
    public class TestPurposeMappingProfile : Profile
    {
        public TestPurposeMappingProfile()
        {
            CreateMap<TestPurpose, TestPurposeResponseDTO>();
        }
    }
}
