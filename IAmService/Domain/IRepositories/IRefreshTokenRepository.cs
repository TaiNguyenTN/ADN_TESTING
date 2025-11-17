using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.IRepositories
{
    public interface IRefreshTokenRepository : IRepositoryBase
    {
        Task<string> AddTokenAsync(Guid userId);
        Task<string?> GetByTokenAsync(string token);
    }
}
