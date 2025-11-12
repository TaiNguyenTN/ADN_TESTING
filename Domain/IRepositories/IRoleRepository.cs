using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.IRepositories
{
    public interface IRoleRepository : IGenericRepository<Role>, IRepositoryBase
    {
        Task<Role?> GetByCodeAsync(string code);
    }
}
