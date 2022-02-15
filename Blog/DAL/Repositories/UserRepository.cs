using Blog.Data;
using Blog.Data.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public sealed class UserRepository : Repository<User>
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public override async Task<User> GetAsync(Expression<Func<User, bool>> expression)
        {
            return await _context.Users.SingleOrDefaultAsync(expression);
        }

        public override async Task InsertAsync(User user)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username);

            // check if user with the same Username exists
            if (dbUser is null)
            {
                await _context.Users.AddAsync(user);
            }
            else
            {
                throw new ArgumentException($"User {user.Username} already exists.", nameof(user));
            }
        }

        public override void Update(User user)
        {
            _context.Users.Update(user);
        }

        public override async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            _context.Users.Remove(user);
        }
    }
}
