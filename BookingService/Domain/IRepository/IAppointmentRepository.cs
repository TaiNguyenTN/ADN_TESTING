using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>, IRepositoryBase
    {
        Task<List<Appointment>> GetAppointmentsByIdentityNumberAsync(string identityNumber);
    }
}
