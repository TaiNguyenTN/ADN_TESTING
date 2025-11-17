using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.IRepositories;
using Infrastructure.Data;
using Infrastructure.InfrastructureException;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Attributes
        protected readonly IAMDBContext _context;
        #endregion
        public GenericRepository(IAMDBContext context)
        {
            _context = context;
        }

        public void AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Remove(Guid id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync() ?? Enumerable.Empty<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> Update(Guid id, T entity)
        {
            var existingEntity = await _context.Set<T>().FindAsync(id);
            if (existingEntity == null)
                throw new RepositoryException($"Entity with ID:{id} is not found");

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            return existingEntity;

        }
    }
}
