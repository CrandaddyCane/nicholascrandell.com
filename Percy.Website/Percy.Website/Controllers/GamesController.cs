
using Microsoft.AspNetCore.Mvc;

namespace Percy.Website.Controllers
{
    [Route("workspace/games")]
    public class GamesController : Controller
    {
        // Video Game Trackers dashboard
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
