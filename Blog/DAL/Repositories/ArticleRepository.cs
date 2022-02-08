using Blog.DAL.Interfaces;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public class ArticleRepository : IRepository<Article>
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Article.ToListAsync();
        }

        public async Task<Article> GetByIdAsync(int id)
        {
            return await _context.Article.FindAsync(id);
        }

        public async Task InsertAsync(Article article)
        {
            await _context.Article.AddAsync(article);
        }

        public void Update(Article article)
        {
            _context.Article.Update(article);
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _context.Article.FindAsync(id);

            _context.Article.Remove(article);
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
