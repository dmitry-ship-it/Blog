﻿using Blog.DAL.Interfaces;
using Blog.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        protected Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public abstract Task<IEnumerable<T>> GetAllAsync();

        public abstract Task<T> GetByIdAsync(int id);

        public abstract Task InsertAsync(T obj);

        public abstract void Update(T obj);

        public abstract Task DeleteAsync(int id);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
