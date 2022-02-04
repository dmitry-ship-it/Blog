using Blog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Identity.Pages.Account.Manage
{
    public class AdminPanelModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ChangePasswordModel> _logger;

        public AdminPanelModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext dbContext,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(256, MinimumLength = 3)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [Display(Name = "Choose a new role")]
            public string NewRole { get; set; }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.Username == User.Identity.Name)
            {
                StatusMessage = "You cannot change your role.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _dbContext.Users.Single(user => user.UserName == Input.Username);
            if (user is null)
            {
                _logger.LogInformation($"User \"{Input.Username}\" was not found.");
                return NotFound($"Unable to load user with Username '{Input.Username}'.");
            }

            _logger.LogInformation($"User \"{Input.Username}\" found successfully.");

            var userRoleListResult = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRoleAsync(user, userRoleListResult[0]);
            await _userManager.AddToRoleAsync(user, Input.NewRole);

            _logger.LogInformation("Role changed successfully.");
            StatusMessage = "Role changed successfully.";

            return RedirectToPage();
        }
    }
}
