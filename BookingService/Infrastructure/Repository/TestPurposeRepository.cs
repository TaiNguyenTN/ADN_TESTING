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
    public class TestPurposeRepository : GenericRepository<TestPurpose> ,ITestPurposeRepository
    {
        public TestPurposeRepository(BookDbContext context) : base(context)
        {
        }

        public Task<TestPurpose?> GetTestPurposeByNameAsync(string purposeName)
        {
            return context.Set<TestPurpose>()
                           .FirstOrDefaultAsync(tp => tp.TestPurposeName == purposeName);
        }
    }
}
