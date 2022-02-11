using Blog.Data;
using Blog.Data.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public override async Task<Article> GetByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
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
        }
    }
}
