using Blog.DAL.Repositories;
using Blog.Data.DbModels;
using Blog.Data.Validation;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [AllowAnonymous]
    public class ArticlesController : Controller
    {
        private readonly Repository<Article> _articleRepository;

        private readonly Repository<Comment> _commentRepository;

        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(Repository<Article> articleRepository,
            Repository<Comment> commentRepository,
            ILogger<ArticlesController> logger)
        {
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var articles = await _articleRepository.GetAllAsync();

            return View(articles.Select(item => (ArticleViewModel)item));
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var article = await _articleRepository.GetAsync(article => article.Id == id);

            if (article is null)
            {
                return NotFound();
            }

            return View((ArticleViewModel)article);
        }

        // GET: Articles/Create
        [RequireAuthorization]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        [HttpPost]
        [RequireAuthorization]
        public async Task<IActionResult> Create(CreateArticleModel model)
        {
            await _articleRepository.InsertAsync(new Article()
            {
                Title = model.Title,
                Text = model.Text,
                Date = model.Date,
                Username = model.Username
            });

            _logger.LogInformation("New article is created by {Username}.", model.Username);

            return RedirectToAction(nameof(Index));
        }

        // GET: Articles/Edit/5
        [RequireAuthorization]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var article = await _articleRepository.GetAsync(article => article.Id == id);

            if (article is null)
            {
                return NotFound();
            }

            if (article.Username != HttpContext.User.Identity.Name
                && !HttpContext.User.IsInRole(nameof(Role.Admin)))
            {
                return Unauthorized();
            }

            return View(new EditArticleModel()
            {
                Title = article.Title,
                Text = article.Text,
                Date = article.Date,
                Username = article.Username,
                Comments = article.Comments.Select(item => (CommentViewModel)item).ToList()
            });
        }

        // POST: Articles/Edit/5
        [HttpPost]
        [RequireAuthorization]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditArticleModel model)
        {
            try
            {
                var article = new Article()
                {
                    Id = id,
                    Title = model.Title,
                    Text = model.Text,
                    Date = model.Date,
                    Username = model.Username,
                    Comments = model.Comments.Select(item => (Comment)item).ToList()
                };

                await _articleRepository.UpdateAsync(article);
            }
            catch (DbUpdateConcurrencyException) when (!ArticleExists(id))
            {
                _logger.LogWarning("Article (id = {Id}) is not changed (not found).", id);

                return NotFound();
            }

            _logger.LogInformation("Article (id = {Id}) is changed.", id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Articles/Delete/5
        [RequireAuthorization]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var article = await _articleRepository.GetAsync(article => article.Id == id);

            if (article is null)
            {
                return NotFound();
            }

            if (article.Username != HttpContext.User.Identity.Name
                && !User.IsInRole(nameof(Role.Admin)))
            {
                return Unauthorized();
            }

            return View((ArticleViewModel)article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [RequireAuthorization]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _articleRepository.DeleteAsync(id);

            _logger.LogInformation("Article (id = {Id}) deleted.", id);

            return RedirectToAction(nameof(Index));
        }

        // POST: Articles/AddComment
        [HttpPost]
        [RequireAuthorization]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(AddCommentModel model)
        {
            if (ModelState.IsValid)
            {
                await _commentRepository.InsertAsync(new Comment()
                {
                    Text = model.Text,
                    Username = model.Username,
                    DateCreated = model.Date,
                    ArticleId = model.ArticleId,
                    OutsideCommentId = model.OutsideCommentId
                });

                _logger.LogInformation("New comment added by {Username} to article id = {ArticleId}.",
                    model.Username, model.ArticleId);
            }
            else
            {
                ModelState.TryAddModelError(string.Empty, "Error on adding comment.");
                _logger.LogWarning("An error occurred while adding a comment (model state is invalid).");
            }

            return RedirectToAction("Details", new { id = model.ArticleId });
        }

        // POST: Articles/DeleteComment
        [HttpPost]
        [RequireAuthorization]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepository.GetAsync(comment => comment.Id == id);

            if (comment is null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteAsync(comment.Id);

            _logger.LogInformation("Comment (id = {Id}) and its replies deleted by {Username}.",
                comment.Id, HttpContext.User.Identity.Name);

            return RedirectToAction("Details", new { id = comment.ArticleId });
        }

        [NonAction]
        private bool ArticleExists(int id)
        {
            return _articleRepository.GetAsync(comment => comment.Id == id) != null;
        }
    }
}
