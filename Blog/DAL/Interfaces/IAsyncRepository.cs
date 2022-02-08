using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.DAL.Interfaces
{
    public interface IAsyncRepository<T> : IAsyncDisposable
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task InsertAsync(T obj);

        Task UpdateAsync(T obj);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}
