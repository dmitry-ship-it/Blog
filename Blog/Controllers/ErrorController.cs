using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Blog.Controllers
{
    public class ErrorController : Controller
    {
        [Route("[controller]")]
        public IActionResult Index()
        {
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
                return Redirect("/Error");
            }

            return View(new ErrorViewModel()
            {
                RequestId = Activity.Current.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code.Value
            });
        }
    }
}
