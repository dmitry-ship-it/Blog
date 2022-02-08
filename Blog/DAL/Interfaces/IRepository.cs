using System;
using System.Collections.Generic;

namespace Blog.DAL.Interfaces
{
    public interface IRepository<T> : IRegularRepository<T>, IAsyncRepository<T>
    {
    }
}
