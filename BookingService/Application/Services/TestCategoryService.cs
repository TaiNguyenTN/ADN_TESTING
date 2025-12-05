using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interface;
using Application.Models.TestCategoryDTOs;
using AutoMapper;
using Domain.IRepository;

namespace Application.Services
{
    public class TestCategoryService : ITestCategoryService
    {
        private readonly ITestCategoryRepository _testCategoryRepository;
        private readonly IMapper mapper;
        public TestCategoryService(ITestCategoryRepository testCategoryRepository, IMapper mapper)
        {
            _testCategoryRepository = testCategoryRepository;
            this.mapper = mapper;
        }
        public async Task<GenericResult<IEnumerable<TestCategoryResponseDTO>>> GetAllTestCategoriesAsync()
        {
            var testCategories = await _testCategoryRepository.GetAllAsync();
            var testCategoryDTOs = mapper.Map<IEnumerable<TestCategoryResponseDTO>>(testCategories);
            return GenericResult<IEnumerable<TestCategoryResponseDTO>>.Success(testCategoryDTOs, "Test categories retrieved successfully.");
        }
    }
}
