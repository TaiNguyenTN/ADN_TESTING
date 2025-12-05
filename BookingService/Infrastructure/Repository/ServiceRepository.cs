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
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(BookDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await context.Services
                        .Include(s => s.TestCategories)
                        .Include(s => s.ServiceTestPurposes)
                            .ThenInclude(stp => stp.TestPurpose)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<Service?> GetServiceByNameAsync(string serviceName)
        {
             return await context.Services
                                      .FirstOrDefaultAsync(s => s.ServiceName == serviceName);
        }
    }
}
