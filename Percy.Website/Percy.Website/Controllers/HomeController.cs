
using Microsoft.AspNetCore.Mvc;
using Percy.Website.Models;
using System.Diagnostics;

namespace Percy.Website.Controllers
{
    public class HomeController : Controller
    {
        // Homepage
        public IActionResult Index()
        {
            return View();
        }

        // Portfolio and project showcase
        public IActionResult Portfolio()
        {
            return View();
        }

        // Login page
        // Authentication will be implemented in a later phase.
        public IActionResult Login()
        {
            return View();
        }

        // Contact page
        // Form processing will be implemented separately.
        public IActionResult Contact()
        {
            return View();
        }

        // Resume
        // Document delivery will be implemented later.
        public IActionResult Resume()
        {
            return View();
        }

        // Privacy policy
        public IActionResult Privacy()
        {
            return View();
        }

        // Application error handling
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
