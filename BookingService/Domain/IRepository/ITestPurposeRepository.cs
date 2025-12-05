using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface ITestPurposeRepository : IGenericRepository<TestPurpose>, IRepositoryBase
    {
        Task<TestPurpose?> GetTestPurposeByNameAsync(string purposeName);
    }
}
