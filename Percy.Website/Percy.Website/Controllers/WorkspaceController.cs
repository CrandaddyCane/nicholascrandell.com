
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Percy.Website.Controllers
{
    [Authorize]
    public class WorkspaceController : Controller
    {
        // Homepage for the private workspace.
        // Requires an authenticated user.
        public IActionResult Index()
        {
            return View();
        }
    }
}
