using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Models.TestCategoryDTOs;

namespace Application.Interface
{
    public interface ITestCategoryService
    {
        Task<GenericResult<IEnumerable<TestCategoryResponseDTO>>> GetAllTestCategoriesAsync();
    }
}
