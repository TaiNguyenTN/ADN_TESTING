using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly BookDbContext context;

        public GenericRepository(BookDbContext context)
        {
            this.context = context;
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await context.Set<T>().AsNoTracking().ToListAsync() ?? Enumerable.Empty<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task RemoveAsync(Guid id)
        {
            var entity = await context.Set<T>().FindAsync(id);
            if (entity != null)
                context.Set<T>().Remove(entity);
        }

        public async Task<T?> Update(Guid id, T entity)
        {
            var existingEntity = await context.Set<T>().FindAsync(id);
            if (existingEntity == null) return null;

            context.Entry(existingEntity).CurrentValues.SetValues(entity);
            return existingEntity!;
        }
    }
}
