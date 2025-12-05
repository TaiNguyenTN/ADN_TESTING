using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class TestCategoryRepository : GenericRepository<TestCategory> ,ITestCategoryRepository
    {
        public TestCategoryRepository(BookDbContext context) : base(context)
        {
        }

        public async Task<TestCategory?> GetTestCategoryByNameAsync(string categoryName)
        {
            return await context.TestCategories
                .FirstOrDefaultAsync(tc => tc.CategoryName == categoryName);
        }
    }
}
