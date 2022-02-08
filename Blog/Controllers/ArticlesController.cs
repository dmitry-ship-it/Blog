using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Blog.DAL.Interfaces;
using Blog.DAL.Repositories;

#pragma warning disable S4144 // Methods should not have identical implementations

namespace Blog.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IRepository<Article> _articleRepository;

        private readonly IRepository<Comment> _commentRepository;

        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(ILogger<ArticlesController> logger, ApplicationDbContext dbContext)
        {
            _articleRepository = new ArticleRepository(dbContext);
            _commentRepository = new CommentRepository(dbContext);
            _logger = logger;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            return View(await _articleRepository.GetAllAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var article = await _articleRepository.GetByIdAsync(id.Value);

            if (article is null)
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
                await _articleRepository.InsertAsync(article);
                await _articleRepository.SaveAsync();

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
            if (id is null)
            {
                return NotFound();
            }

            var article = await _articleRepository.GetByIdAsync(id.Value);

            if (article is null)
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
                    await _articleRepository.UpdateAsync(article);
                    await _articleRepository.SaveAsync();
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

            var article = await _articleRepository.GetByIdAsync(id.Value);

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
            await _articleRepository.DeleteAsync(id);
            await _articleRepository.SaveAsync();

            _logger.LogInformation($"Article (id = {id}) deleted.");

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
                await _commentRepository.InsertAsync(comment);
                await _commentRepository.SaveAsync();

                _logger.LogInformation($"Comment (id = {comment.Id}) added.");
            }
            else
            {
                _logger.LogWarning($"Comment (id = {comment.Id}) is not added (model state is invalid).");
            }

            return RedirectToAction("Details", new { id = comment!.ArticleId });
        }

        // POST: Articles/DeleteComment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);

            if (comment is null)
            {
                return NotFound();
            }

            await RemoveCommentAndRepliesRecursive(comment);
            await _commentRepository.SaveAsync();

            _logger.LogInformation($"Comment (id = {comment.Id}) and its replies deleted.");

            return RedirectToAction("Details", new { id = comment.ArticleId });
        }

        private async Task RemoveCommentAndRepliesRecursive(Comment comment)
        {
            if (comment.Replies.Count != 0)
            {
                foreach (var reply in comment.Replies)
                {
                    await RemoveCommentAndRepliesRecursive(reply);
                }
            }

            await _commentRepository.DeleteAsync(comment.Id);
        }

        private bool ArticleExists(int id)
        {
            return _articleRepository.GetById(id) != null;
        }
    }
}
