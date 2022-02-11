using Blog.Data;
using Blog.Data.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public sealed class CommentRepository : Repository<Comment>
    {
        public CommentRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }

        public override async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public override async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public override async Task InsertAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public override void Update(Comment comment)
        {
            _context.Comments.Update(comment);
        }

        public override async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            _context.Comments.Remove(comment);
        }
    }
}
