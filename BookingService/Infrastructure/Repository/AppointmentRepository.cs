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
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(BookDbContext context) : base(context)
        {
        }

        public Task<List<Appointment>> GetAppointmentsByIdentityNumberAsync(string identityNumber)
        {
            return context.Appointments.AsNoTracking()
                .Where(a => a.IdentityNumber == identityNumber) // dùng field IdentityNumber mới
                .ToListAsync();
        }

    }
}
