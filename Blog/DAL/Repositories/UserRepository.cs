using Blog.Data;
using Blog.Data.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public override async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public override async Task InsertAsync(User user)
        {
            await _context.Users.AddAsync(user);
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
