using Blog.Data;
using Blog.Data.DbModels;
using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public sealed class ArticleRepository : Repository<Article>
    {
        public ArticleRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        { }

        public override async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public override async Task<Article> GetAsync(Expression<Func<Article, bool>> expression)
        {
            return await _context.Articles.SingleOrDefaultAsync(expression);
        }

        public override async Task InsertAsync(Article article)
        {
            await _context.Articles.AddAsync(article);
        }

        public override void Update(Article article)
        {
            _context.Articles.Update(article);
        }

        public override async Task DeleteAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            _context.Articles.Remove(article);

            foreach (var comment in article.Comments)
            {
                _context.Comments.Remove(comment);
            }
        }
    }
}
