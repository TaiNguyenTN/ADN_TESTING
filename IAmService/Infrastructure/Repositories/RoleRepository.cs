using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        
        public RoleRepository(IAMDBContext context) : base(context)
        {
        }

        public async Task<Role?> GetByCodeAsync(string code)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rp => rp.Privilege)
                .FirstOrDefaultAsync(r => r.Code == code);

            //var role = await _context.Roles
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(r => r.Code == code);
            return role;
        }
    }
}
