
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Percy.Website.Data;

namespace Percy.Website.Controllers
{
    // Temporary controller for testing SQL Server connectivity.
    // Remove this controller after database integration is verified.
    [Route("workspace/database-test")]
    public class DatabaseTestController : Controller
    {
        private readonly PercyWorkspaceContext _context;

        public DatabaseTestController(PercyWorkspaceContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsDevelopment())
            {
                return NotFound();
            }

            var fish = await _context.MuseumItems
                .AsNoTracking()
                .Where(item =>
                    item.GameID == 1 &&
                    item.CategoryID == 1)
                .OrderBy(item => item.ItemName)
                .Select(item => new
                {
                    item.MuseumItemID,
                    item.ItemName,
                    item.SellPrice
                })
                .ToListAsync();

            return Json(new
            {
                Database = "PercyWorkspace",
                FishCount = fish.Count,
                Fish = fish
            });
        }
    }
}
