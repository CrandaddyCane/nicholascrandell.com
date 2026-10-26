
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Percy.Website.Data;
using Percy.Website.Models;

namespace Percy.Website.Controllers
{
    [Route("workspace/animal-crossing")]
    public class AnimalCrossingController : Controller
    {
        private readonly PercyWorkspaceContext _context;

        // Temporary player selection for local development.
        private const int GameID = 1;
        private const int FishCategoryID = 1;
        private const int TownID = 1;
        private const int PlayerID = 1;

        public AnimalCrossingController(
            PercyWorkspaceContext context)
        {
            _context = context;
        }

        // Display the Fish collection.
        [HttpGet("fish")]
        public async Task<IActionResult> Fish()
        {
            var donatedIds = await _context.MuseumDonations
                .AsNoTracking()
                .Where(d => d.TownID == TownID)
                .Select(d => d.MuseumItemID)
                .ToListAsync();

            var donatedSet = donatedIds.ToHashSet();

            // Load fish spawning locations.
            var locationRecords = await (
                from fsl in _context.FishSpawnLocations
                join sl in _context.SpawnLocations
                    on fsl.LocationID equals sl.LocationID
                select new
                {
                    fsl.MuseumItemID,
                    sl.LocationName
                }
            ).AsNoTracking().ToListAsync();

            var locationsByFish = locationRecords
                .GroupBy(x => x.MuseumItemID)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ",
                        g.Select(x => x.LocationName)
                         .OrderBy(name => name))
                );

            // Retrieve fish, including shadow sizes.
            var fish = await _context.MuseumItems
                .AsNoTracking()
                .Where(item =>
                    item.GameID == GameID &&
                    item.CategoryID == FishCategoryID &&
                    item.IsActive)
                .OrderBy(item => item.ItemName)
                .Select(item => new FishViewModel
                {
                    MuseumItemID = item.MuseumItemID,
                    FishName = item.ItemName,
                    SellPrice = item.SellPrice,
                    ShadowSize = item.ShadowSize
                })
                .ToListAsync();

            foreach (var item in fish)
            {
                item.IsDonated =
                    donatedSet.Contains(item.MuseumItemID);

                item.SpawnLocations =
                    locationsByFish.TryGetValue(
                        item.MuseumItemID,
                        out var locations)
                        ? locations
                        : string.Empty;
            }

            return View(fish);
        }

        // Save a change to a donation checkbox.
        [HttpPost("fish/donation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFishDonation(
            int museumItemId,
            bool isDonated)
        {
            var fishExists = await _context.MuseumItems
                .AnyAsync(item =>
                    item.MuseumItemID == museumItemId &&
                    item.GameID == GameID &&
                    item.CategoryID == FishCategoryID &&
                    item.IsActive);

            if (!fishExists)
            {
                return NotFound();
            }

            var existingDonation = await _context.MuseumDonations
                .FirstOrDefaultAsync(d =>
                    d.TownID == TownID &&
                    d.MuseumItemID == museumItemId);

            if (isDonated && existingDonation == null)
            {
                _context.MuseumDonations.Add(
                    new MuseumDonation
                    {
                        TownID = TownID,
                        PlayerID = PlayerID,
                        MuseumItemID = museumItemId,
                        DonationDate = null
                    });
            }
            else if (!isDonated && existingDonation != null)
            {
                _context.MuseumDonations.Remove(existingDonation);
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                museumItemId,
                isDonated
            });
        }
    }
}
