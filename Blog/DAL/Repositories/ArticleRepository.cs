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

        public IEnumerable<Article> GetAll()
        {
            return _context.Article.ToList();
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Article.ToListAsync();
        }

        public Article GetById(int id)
        {
            return _context.Article.Find(id);
        }

        public async Task<Article> GetByIdAsync(int id)
        {
            return await _context.Article.FindAsync(id);
        }

        public void Insert(Article article)
        {
            _context.Article.Add(article);
        }

        public async Task InsertAsync(Article article)
        {
            await _context.Article.AddAsync(article);
        }

        public void Update(Article article)
        {
            _context.Article.Update(article);
        }

        public async Task UpdateAsync(Article article)
        {
            await Task.Run(() => _context.Article.Update(article));
        }

        public void Delete(int id)
        {
            var article = _context.Article.Find(id);

            _context.Article.Remove(article);
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _context.Article.FindAsync(id);

            await Task.Run(() => _context.Article.Remove(article));
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
