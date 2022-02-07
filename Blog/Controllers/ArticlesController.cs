using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Blog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public ArticlesController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Article.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Title,Text,Date")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.Date = DateTime.Now;
                _context.Add(article);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Article (id = {article.Id}) is created.");

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning($"Article (id = {article.Id}) is not created.");

            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Text,Date")] Article article)
        {
            if (id != article?.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!ArticleExists(article.Id))
                {
                    _logger.LogWarning($"Article (id = {article.Id}) is not changed (not found).");

                    return NotFound();
                }

                _logger.LogInformation($"Article (id = {article.Id}) is changed.");

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning($"Article (id = {article.Id}) is not changed (model state invalid).");

            return View(article);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article is null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Article (id = {article.Id}) deleted.");

            return RedirectToAction(nameof(Index));
        }

        // POST: Articles/AddComment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([Bind("Id,Username,Text,DateCreated,ArticleId,CommentId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Comment (id = {comment.Id}) added.");
            }
            else
            {
                _logger.LogInformation($"Comment (id = {comment.Id}) is not added (model state is invalid).");
            }

            return RedirectToAction("Details", new { id = comment!.ArticleId });
        }

        // POST: Articles/DeleteComment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comment.FindAsync(id);

            if (comment is null)
            {
                return NotFound();
            }

            RemoveCommentAndRepliesRecursive(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Comment (id = {comment.Id}) and its replies deleted.");

            return RedirectToAction("Details", new { id = comment.ArticleId });
        }

        private void RemoveCommentAndRepliesRecursive(Comment comment)
        {
            if (comment.Replies.Count != 0)
            {
                foreach (var reply in comment.Replies)
                {
                    RemoveCommentAndRepliesRecursive(reply);
                }
            }

            _context.Comment.Remove(comment);
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
