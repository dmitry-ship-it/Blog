using Blog.DAL.Interfaces;
using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public class UserRepository : IRepository<IdentityUser>
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public IEnumerable<IdentityUser> GetAll()
        {
            return _context.Users.ToList();
        }

        public async Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public IdentityUser GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public async Task<IdentityUser> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public void Insert(IdentityUser user)
        {
            _context.Users.Add(user);
        }

        public async Task InsertAsync(IdentityUser user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(IdentityUser user)
        {
            _context.Users.Update(user);
        }

        public async Task UpdateAsync(IdentityUser user)
        {
            await Task.Run(() => _context.Users.Update(user));
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);

            _context.Users.Remove(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            await Task.Run(() => _context.Users.Remove(user));
        }

        public void Save()
        {
            _context.SaveChanges();
        }

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

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed && disposing && _context != null)
            {
                await _context.DisposeAsync();
            }

            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);

            GC.SuppressFinalize(this);
        }
    }
}
