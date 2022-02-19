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

        public UserController(ILogger<ArticlesController> logger,
            Repository<User> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
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
                return View();
            }

            // check user password
            if (!UserProtector.CheckCredentials(userSearchResult, model.Password))
            {
                return View();
            }

            CreateAuthenticationTicket(userSearchResult);
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
                return BadRequest();
            }

            var newUser = UserProtector.CreateUser(model.Username, model.Password);
            await _userRepository.InsertAsync(newUser);

            _logger.LogInformation($"User '{model.Username}' registered successfully.");

            return RedirectToAction("Index", "Home");
        }

        private void CreateAuthenticationTicket(User user)
        {
            var key = Encoding.ASCII.GetBytes(SiteKeys.Token);
            var JWToken = new JwtSecurityToken(
                issuer: SiteKeys.WebSiteDomain,
                audience: SiteKeys.WebSiteDomain,
                claims: GetUserClaims(user),
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            HttpContext.Session.SetString("JWToken", token);
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>();
            var claim = new Claim(ClaimTypes.Name, user.Username);
            claims.Add(claim);

            claim = new Claim(ClaimTypes.Role, user.Role.ToString());
            claims.Add(claim);

            // if user has a specific role
            // add default 'User'
            if (user.Role != Role.User)
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Role.User)));
            }

            return claims;
        }
    }
}
