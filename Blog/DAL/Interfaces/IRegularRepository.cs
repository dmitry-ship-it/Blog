using System;
using System.Collections.Generic;

namespace Blog.DAL.Interfaces
{
    public interface IRegularRepository<T> : IDisposable
    {
        IEnumerable<T> GetAll();

        T GetById(int id);

        void Insert(T obj);

        void Update(T obj);

        void Delete(int id);

        void Save();
    }
}
