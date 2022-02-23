using Blog.Data;
using Blog.Data.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public override async Task<Comment> GetAsync(Expression<Func<Comment, bool>> expression)
        {
            return await _context.Comments.SingleOrDefaultAsync(expression);
        }

        public override async Task InsertAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public override async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            await RemoveCommentAndRepliesRecursive(comment);

            await _context.SaveChangesAsync();
        }

        private async Task RemoveCommentAndRepliesRecursive(Comment comment)
        {
            if (comment.Replies?.Count != 0)
            {
                foreach (var reply in comment.Replies)
                {
                    await RemoveCommentAndRepliesRecursive(reply);
                }
            }

            _context.Remove(comment);
        }
    }
}
