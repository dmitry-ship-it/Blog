using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.DAL.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByKeyValuesAsync(params object[] keyValues);

        Task InsertAsync(T obj);

        void Update(T obj);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}
