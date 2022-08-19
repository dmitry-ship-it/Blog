using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Blog.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("[controller]")]
        public IActionResult Index()
        {
            _logger.LogWarning("Unknown error occurred.");

            return View(new ErrorViewModel()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [ActionName("Index")]
        [Route("[controller]/{code?}")]
        public IActionResult IndexWithErrorCode(int? code)
        {
            if (!code.HasValue || code < 400)
            {
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Error {ErrorCode} occurred.", code);

            return View(new ErrorViewModel()
            {
                RequestId = Activity.Current.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code.Value
            });
        }
    }
}
