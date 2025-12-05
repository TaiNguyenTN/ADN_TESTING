using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IServiceRepository : IGenericRepository<Service>, IRepositoryBase
    {
        Task<Service?> GetServiceByNameAsync(string serviceName);
        Task<IEnumerable<Service>> GetAllServicesAsync();
    }
}
