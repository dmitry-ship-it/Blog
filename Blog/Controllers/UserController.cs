using Blog.DAL.Repositories;
using Blog.Data.DbModels;
using Blog.Data.Validation;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly Repository<User> _userRepository;

        private readonly ILogger<ArticlesController> _logger;

        private readonly UserManager _userManager;

        public UserController(ILogger<ArticlesController> logger,
            Repository<User> userRepository,
            UserManager userManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        // GET: /User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginConfirmed(LoginModel model)
        {
            var userSearchResult = await _userRepository.GetAsync(user => user.Username == model.Username);

            // check user password
            if (userSearchResult is null
                || !_userManager.CheckCredentials(userSearchResult, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username or password is invalid.");
                return View();
            }

            _userManager.CreateAuthenticationTicket(userSearchResult, HttpContext.Session);

            _logger.LogInformation($"User '{model.Username}' logged in.");

            return RedirectPermanent("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectPermanent("/");
        }

        // GET: /User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> RegisterConfirmed(RegisterModel model)
        {
            if (model.Username == model.Password)
            {
                ModelState.AddModelError(string.Empty, "You cannot use your username as password.");
                return View();
            }

            var userSearchResult = await _userRepository.GetAsync(user => user.Username == model.Username);

            // if user already exists
            if (userSearchResult != null)
            {
                ModelState.TryAddModelError(string.Empty, "User with this username already exists.");
                return View();
            }

            var newUser = _userManager.CreateUser(model.Username, model.Password);
            await _userRepository.InsertAsync(newUser);

            _logger.LogInformation($"User '{model.Username}' registered successfully.");

            return RedirectToActionPermanent(nameof(Created));
        }

        // GET: /User/Created
        public IActionResult Created()
        {
            return View();
        }
    }
}
