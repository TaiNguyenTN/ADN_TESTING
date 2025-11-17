using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Attributes
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, IRepositoryBase> repositories = new();

        private readonly IAMDBContext _context;
        private IDbContextTransaction? _transaction;
        #endregion

        public UnitOfWork(IServiceProvider provider, IAMDBContext context)
        {
            _provider = provider;
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)   //Kiểm tra xem hiện tại đang có transaction nào hoạt động ko? 
            {   //Nếu đã có (đang chạy), thì không tạo thêm (tránh mở lặp transaction). Nếu chưa có, thì mới tạo mới transaction.
                // Điều này giúp tránh tình huống “một UnitOfWork lồng nhiều transaction”, gây lỗi hoặc không cần thiết.
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task<int> CommitAsync(string? perfomedBy = null)
        {
            try
            {
                await AddAuditLogsAsync(perfomedBy);

                int changed = await _context.SaveChangesAsync();

                if(_transaction != null)
                {
                    await _transaction.CommitAsync();
                }

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

        private async Task RollbackAsync()
        {
            if (_transaction != null) //Kiểm tra xem hiện tại có transaction phải đang hoạt động
            {
                await _transaction.RollbackAsync();
                //Đây là lệnh hoàn tác tất cả các thay đổi trong transaction hiện tại.
                //Entity Framework Core gửi lệnh ROLLBACK TRANSACTION xuống database (SQL Server, MySQL, PostgreSQL, v.v.).
                //Mọi thay đổi chưa được commit sẽ bị bỏ hết (không lưu xuống DB).

                await _transaction.DisposeAsync();
                //Sau khi rollback xong, giải phóng tài nguyên của transaction khỏi bộ nhớ.
                _transaction = null;
                //Đặt biến _transaction về null để đánh dấu rằng: Hiện tại không còn transaction nào đang mở nữa.
            }
        }

        private async Task AddAuditLogsAsync(string? performedBy)   //Tự động ghi log mỗi khi dữ liệu trong DbContext bị thêm, sửa hoặc xóa.
        /*
        Cụ thể, nó sẽ:
        1. Lấy ra danh sách các entity đang bị thay đổi trong DbContext.
        2. Xác định loại hành động (Added, Modified, Deleted).
        3. Trích xuất dữ liệu cũ (oldValue) và dữ liệu mới (newValue).
        4. Ghi vào bảng AuditLog.
        */
        {
            var entries = _context.ChangeTracker.Entries()      //là cơ chế theo dõi(tracking) của EntityFrameworkCore
                .Where(e => e.State == EntityState.Added ||     //Phương thức .Entries() trả về danh sách các đối tượng đang
                                                                //được theo dõi, mỗi phần tử là một EntityEntry, chứa thông
                                                                //tin như:
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted).ToList();
            //entries là danh sách các EntityEntry, mỗi cái chứa thông tin về 1 entity thay đổi.
            foreach (var entry in entries)
            {
                string entityName = entry.Entity.GetType().Name;
                //entry.Entity: chính object entity bị thay đổi (ví dụ: User, Order, Product...).
                string action = entry.State.ToString();

                string? oldValue = entry.State == EntityState.Added
                    ? null 
                    : JsonSerializer.Serialize(entry.OriginalValues.ToObject());

                string? newValue = entry.State == EntityState.Deleted
                    ? null
                    : JsonSerializer.Serialize(entry.CurrentValues.ToObject());

                var auditLog = new AuditLog(
                    entityName: entityName,
                    action: action,
                    performedBy: performedBy,
                    oldValue: oldValue,
                    newValue: newValue);

                GetRepository<IAuditLogRepository>().AddAsync(auditLog);
            }
        }

        public T GetRepository<T>() where T : IRepositoryBase
        {
            var type = typeof(T);
            if(!repositories.TryGetValue(type, out var repo))
            //Nếu dictionary repositories đã có repository kiểu type này → lấy ra repo.
            //Nếu chưa → tạo mới.
            {

                repo = (IRepositoryBase)_provider.GetRequiredService(type);
                //DI container sẽ trả về instance của UserRepository đã đăng ký cho interface IUserRepository.
                //Cast về IRepositoryBase để lưu trong dictionary.
                repositories[type] = repo;
                //Cache lại repository vào dictionary. Lần sau gọi GetRepository<IUserRepository>() sẽ lấy trực tiếp từ dictionary, không cần gọi DI nữa.
            }

            return (T)repo;// trả về UserRepository

            /*
             Có nghĩa là đầu tiên giả sử T ở đây đang là IUserRepository thì type là đại diện của IUserRepository sau đó sẽ tìm
            trong Dictionary repositories xem có type không nếu có thì lưu vào repo. còn nếu không thì nó sẽ tìm kiếm type trong
            những service đã đăng ký và ép kiểu nó về IRepositoryBase và lưu trong repo tiếp theo nó cache vào dictionary với type
            và repo và cuối cùng trả về repo(UserRepository)
            */
        }
    }
}
