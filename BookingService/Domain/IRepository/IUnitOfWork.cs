using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IUnitOfWork
    {
        T GetRepository<T>() where T : IRepositoryBase;
        Task BeginTransactionAsync();
        Task<int> CommitAsync(string? performedBy = null);
    }

    public interface IRepositoryBase
    {

    }
}
