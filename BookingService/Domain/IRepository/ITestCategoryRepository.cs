using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface ITestCategoryRepository : IGenericRepository<TestCategory>, IRepositoryBase
    {
        Task<TestCategory?> GetTestCategoryByNameAsync(string categoryName);
    }
}
