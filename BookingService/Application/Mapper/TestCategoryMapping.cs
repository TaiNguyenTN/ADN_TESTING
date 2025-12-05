using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.TestCategoryDTOs;
using AutoMapper;
using Domain.Aggregate;

namespace Application.Mapper
{
    public class TestCategoryMapping : Profile
    {
        public TestCategoryMapping()
        {
            CreateMap<TestCategory, TestCategoryResponseDTO>();
        }
    }
}
