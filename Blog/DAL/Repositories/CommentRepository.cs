using Blog.DAL.Interfaces;
using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.DAL.Repositories
{
    public class CommentRepository : IRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public IEnumerable<Comment> GetAll()
        {
            return _context.Comment.ToList();
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comment.ToListAsync();
        }

        public Comment GetById(int id)
        {
            return _context.Comment.Find(id);
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _context.Comment.FindAsync(id);
        }

        public void Insert(Comment comment)
        {
            _context.Comment.Add(comment);
        }

        public async Task InsertAsync(Comment comment)
        {
            await _context.Comment.AddAsync(comment);
        }

        public void Update(Comment comment)
        {
            _context.Comment.Update(comment);
        }

        public async Task UpdateAsync(Comment comment)
        {
            await Task.Run(() => _context.Comment.Update(comment));
        }

        public void Delete(int id)
        {
            var comment = _context.Comment.Find(id);

            _context.Comment.Remove(comment);
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comment.FindAsync(id);

            await Task.Run(() => _context.Comment.Remove(comment));
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
