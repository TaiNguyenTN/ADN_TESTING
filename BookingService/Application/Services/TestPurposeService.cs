using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interface;
using Application.Models.TestPurposeDTOs;
using AutoMapper;
using Domain.IRepository;

namespace Application.Services
{
    public class TestPurposeService : ITestPurposeService
    {
        private readonly ITestPurposeRepository testPurposeRepository;
        private readonly IMapper mapper;
        public TestPurposeService(ITestPurposeRepository testPurposeRepository, IMapper mapper)
        {
            this.testPurposeRepository = testPurposeRepository;
            this.mapper = mapper;
        }
        public async Task<GenericResult<IEnumerable<TestPurposeResponseDTO>>> GetAllTestPurposesAsync()
        {
            var testPurposes = await testPurposeRepository.GetAllAsync();
            var testPurposeDTOs = mapper.Map<IEnumerable<TestPurposeResponseDTO>>(testPurposes);
            return GenericResult<IEnumerable<TestPurposeResponseDTO>>.Success(testPurposeDTOs, "Test categories retrieved successfully.");
        }
    }
}
