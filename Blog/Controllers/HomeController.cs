using Blog.DAL.Repositories;
using Blog.Data.DbModels;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly Repository<Article> _articleRepository;

        public HomeController(Repository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _articleRepository.GetAllAsync();

            return View(articles.Select(item => (ArticleViewModel)item));
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
