using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Models.TestPurposeDTOs;
using AutoMapper;
using Domain.IRepository;

namespace Application.Interface
{
    public interface ITestPurposeService
    {
        Task<GenericResult<IEnumerable<TestPurposeResponseDTO>>> GetAllTestPurposesAsync();
    }
}
