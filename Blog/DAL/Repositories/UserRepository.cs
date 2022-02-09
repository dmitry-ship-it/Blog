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

        public async Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IdentityUser> GetByKeyValuesAsync(params object[] keyValues)
        {
            return await _context.Users.FindAsync(keyValues);
        }

        public async Task InsertAsync(IdentityUser user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(IdentityUser user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            _context.Users.Remove(user);
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
    }
}
