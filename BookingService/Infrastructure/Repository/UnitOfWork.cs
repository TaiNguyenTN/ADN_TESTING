using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, IRepositoryBase> _repositories = new();

        private readonly BookDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(IServiceProvider provider, BookDbContext context)
        {
            _provider = provider;
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            if(_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task<int> CommitAsync(string? performedBy = null)
        {
            try
            {
                await AddAuditLogsAsync(performedBy);

                int changed = await _context.SaveChangesAsync();

                if(_transaction != null)
                    await _transaction.CommitAsync();

                return changed;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if(_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if(_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public T GetRepository<T>() where T : IRepositoryBase
        {
            var type = typeof(T);
            if(!_repositories.TryGetValue(type, out var repo))
            {
                repo = (IRepositoryBase)_provider.GetRequiredService(type);
                _repositories[type] = repo;
            }

            return (T)repo;
        }

        private async Task AddAuditLogsAsync(string? performedBy)
        {
            var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

            foreach(var entry in entries)
            {
                string entityName = entry.Entity.GetType().Name;
                string action = entry.State.ToString();
                string? oldValue = entry.State == EntityState.Added ? null : JsonSerializer.Serialize(entry.OriginalValues.ToObject());

                string? newValue = entry.State == EntityState.Deleted ? null : JsonSerializer.Serialize(entry.CurrentValues.ToObject());

                var auditLog = new AuditLog(
                    entityName: entityName,
                    action: action,
                    performedBy: performedBy,
                    oldValue: oldValue,
                    newValue: newValue);

                GetRepository<IAuditLogRepository>().Add(auditLog);
            }
        }
    }
}
