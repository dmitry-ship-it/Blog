using Blog.DAL.Repositories;
using Blog.Data;
using Blog.Data.DbModels;
using Blog.Data.Validation;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

            if (userSearchResult is null)
            {
                return NotFound();
            }

            // check user password
            if (!_userManager.CheckCredentials(userSearchResult, model.Password))
            {
                return View();
            }

            _userManager.CreateAuthenticationTicket(userSearchResult, HttpContext.Session);

            _logger.LogInformation($"User '{model.Username}' logged in.");

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
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
