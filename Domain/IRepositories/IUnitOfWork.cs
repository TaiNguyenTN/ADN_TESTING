using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepositories
{
    public interface IUnitOfWork
    {
        T GetRepository<T>() where T : IRepositoryBase;
        //Có 2 mục đích quan trọng.
        //1. Bảo đảm kiểu an toàn (type safety) tại thời điểm biên dịch. Chỉ cho phép gọi GetRepository<T>() với những T là
        //repository (kế thừa, implement IRepositoryBase). Nếu không có ràng buộc, bạn vô tình có thể gọi GetRepository<string>()
        // lỗi đó chỉ lộ ra lúc chạy, còn với ràng buộc này, lỗi sẽ được phát hiện ngay khi biên dịch.
        //2. Định nghĩa một hợp đồng rõ ràng cho các repository trong hệ thống.

        Task BeginTransactionAsync();
        Task<int> CommitAsync(string? perfomedBy = null);
    }

    public interface IRepositoryBase
    {
    }
}
