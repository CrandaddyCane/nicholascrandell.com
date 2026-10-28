
using System.Globalization;
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

        // Temporary single-player configuration.
        private const int GameID = 1;
        private const int FishCategoryID = 1;
        private const int InsectCategoryID = 2;
        private const int FossilCategoryID = 3;
        private const int ArtCategoryID = 4;
        private const int TownID = 1;
        private const int PlayerID = 1;

        public AnimalCrossingController(
            PercyWorkspaceContext context)
        {
            _context = context;
        }

        // Read-only dashboard data endpoint.
        // The visual dashboard will be implemented separately.
        [HttpGet("")]
        public async Task<IActionResult> Dashboard(
            string? date,
            string? time,
            string? weather)
        {
            // Initial testing uses server time.
            // The browser will supply local time in the final UI.
            var now = DateTime.Now;
            var selectedDate = DateOnly.FromDateTime(now);
            var selectedTime = TimeOnly.FromDateTime(now);

            if (!string.IsNullOrWhiteSpace(date) &&
                !DateOnly.TryParseExact(
                    date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out selectedDate))
            {
                return BadRequest("Invalid date.");
            }

            if (!string.IsNullOrWhiteSpace(time) &&
                !TimeOnly.TryParseExact(
                    time,
                    new[] { "HH:mm", "HH:mm:ss" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out selectedTime))
            {
                return BadRequest("Invalid time.");
            }

            var validWeather = new[]
            {
                "Unknown",
                "Sunny",
                "Cloudy",
                "Rain",
                "Snow"
            };

            var requestedWeather =
                (weather ?? "Unknown").Trim();

            var selectedWeather = validWeather
                .FirstOrDefault(x =>
                    x.Equals(
                        requestedWeather,
                        StringComparison.OrdinalIgnoreCase));

            if (selectedWeather == null)
            {
                return BadRequest("Invalid weather.");
            }

            var previousDate = selectedDate.AddDays(-1);
            var selectedTimeSpan = selectedTime.ToTimeSpan();

            var items = await _context.MuseumItems
                .AsNoTracking()
                .Where(x =>
                    x.GameID == GameID &&
                    x.IsActive)
                .ToListAsync();

            var donatedIds = (await _context.MuseumDonations
                .AsNoTracking()
                .Where(x => x.TownID == TownID)
                .Select(x => x.MuseumItemID)
                .ToListAsync())
                .ToHashSet();

            var currentMonth = selectedDate.Month;
            var previousMonth = previousDate.Month;

            // Include the prior month's schedule so that
            // overnight availability crossing a month boundary
            // can be evaluated correctly.
            var fishSchedules = await _context.FishAvailabilities
                .AsNoTracking()
                .Where(x =>
                    x.Month == currentMonth ||
                    x.Month == previousMonth)
                .ToListAsync();

            var insectSchedules = await _context.InsectAvailabilities
                .AsNoTracking()
                .Where(x =>
                    x.Month == currentMonth ||
                    x.Month == previousMonth)
                .ToListAsync();

            var fish = BuildSpecies(
                items.Where(x =>
                    x.CategoryID == FishCategoryID),
                fishSchedules.Select(x => new AvailabilityRule
                {
                    MuseumItemID = x.MuseumItemID,
                    Month = x.Month,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    IsAllDay = x.IsAllDay,
                    WeatherCondition = x.WeatherCondition,
                    IsIslandOnly = false
                }),
                donatedIds,
                selectedDate,
                selectedTimeSpan,
                selectedWeather);

            var insects = BuildSpecies(
                items.Where(x =>
                    x.CategoryID == InsectCategoryID),
                insectSchedules.Select(x => new AvailabilityRule
                {
                    MuseumItemID = x.MuseumItemID,
                    Month = x.Month,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    IsAllDay = x.IsAllDay,
                    WeatherCondition = x.WeatherCondition,
                    IsIslandOnly = x.IsIslandOnly
                }),
                donatedIds,
                selectedDate,
                selectedTimeSpan,
                selectedWeather);

            var fossils = items
                .Where(x => x.CategoryID == FossilCategoryID)
                .ToList();

            var art = items
                .Where(x => x.CategoryID == ArtCategoryID)
                .ToList();

            return Json(new
            {
                town = "Maple",
                selectedDate,
                selectedTime,
                weather = selectedWeather,

                collections = new
                {
                    fish = BuildStatistics(fish),
                    insects = BuildStatistics(insects),

                    fossils = new
                    {
                        donated = fossils.Count(x =>
                            donatedIds.Contains(x.MuseumItemID)),
                        total = fossils.Count
                    },

                    art = new
                    {
                        donated = art.Count(x =>
                            donatedIds.Contains(x.MuseumItemID)),
                        total = art.Count
                    }
                },

                species = new
                {
                    fish,
                    insects
                }
            });
        }

        private static List<DashboardSpecies> BuildSpecies(
            IEnumerable<MuseumItem> items,
            IEnumerable<AvailabilityRule> schedules,
            HashSet<int> donatedIds,
            DateOnly selectedDate,
            TimeSpan selectedTime,
            string weather)
        {
            var groupedSchedules = schedules
                .GroupBy(x => x.MuseumItemID)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

            var previousDate = selectedDate.AddDays(-1);

            var result = new List<DashboardSpecies>();

            foreach (var item in items)
            {
                groupedSchedules.TryGetValue(
                    item.MuseumItemID,
                    out var itemSchedules);

                itemSchedules ??= new List<AvailabilityRule>();

                var eligibleRules = itemSchedules
                    .Where(x => WeatherMatches(
                        x.WeatherCondition,
                        weather))
                    .ToList();

                // A species is available today if at least
                // one eligible window overlaps the selected date.
                var availableToday = eligibleRules.Any(x =>
                    IsAvailableDuringDate(
                        x,
                        selectedDate));

                // A species is available now if at least one
                // eligible window includes the selected time.
                var availableNow = eligibleRules.Any(x =>
                    IsAvailableAtTime(
                        x,
                        selectedDate,
                        selectedTime));

                // Show schedules that overlap the selected day.
                var applicableRules = eligibleRules
                    .Where(x =>
                        x.Month == selectedDate.Month ||
                        (x.Month == previousDate.Month &&
                         IsOvernight(x) &&
                         x.EndTime > TimeSpan.Zero))
                    .Where(x => IsAvailableDuringDate(
                        x,
                        selectedDate))
                    .ToList();

                result.Add(new DashboardSpecies
                {
                    MuseumItemID = item.MuseumItemID,
                    Name = item.ItemName,
                    CategoryID = item.CategoryID,
                    SellPrice = item.SellPrice,

                    IsDonated = donatedIds.Contains(
                        item.MuseumItemID),

                    AvailableNow = availableNow,
                    AvailableToday = availableToday,

                    IsIslandOnly = applicableRules.Any(x =>
                        x.IsIslandOnly),

                    AvailabilityWindows = applicableRules
                        .Select(x => x.IsAllDay
                            ? "All day"
                            : $"{FormatTime(x.StartTime)} - " +
                              $"{FormatTime(x.EndTime)}")
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList()
                });
            }

            // Undonated species first, then alphabetical.
            return result
                .OrderBy(x => x.IsDonated)
                .ThenBy(x => x.Name)
                .ToList();
        }

        private static bool IsOvernight(
            AvailabilityRule rule)
        {
            return !rule.IsAllDay &&
                   rule.StartTime > rule.EndTime;
        }

        private static bool IsAvailableDuringDate(
            AvailabilityRule rule,
            DateOnly selectedDate)
        {
            var previousDate = selectedDate.AddDays(-1);

            // Windows beginning on the selected date.
            if (rule.Month == selectedDate.Month)
            {
                if (rule.IsAllDay)
                {
                    return true;
                }

                // Ordinary daytime window.
                if (!IsOvernight(rule))
                {
                    return rule.EndTime > rule.StartTime;
                }

                // Overnight window starts on this date.
                return true;
            }

            // An overnight window beginning yesterday may
            // continue into today.
            return IsOvernight(rule) &&
                   rule.Month == previousDate.Month &&
                   rule.EndTime > TimeSpan.Zero;
        }

        private static bool IsAvailableAtTime(
            AvailabilityRule rule,
            DateOnly selectedDate,
            TimeSpan selectedTime)
        {
            var previousDate = selectedDate.AddDays(-1);

            // All-day schedule for the selected date.
            if (rule.IsAllDay)
            {
                return rule.Month == selectedDate.Month;
            }

            // Ordinary window: start inclusive, end exclusive.
            if (!IsOvernight(rule))
            {
                return rule.Month == selectedDate.Month &&
                       selectedTime >= rule.StartTime &&
                       selectedTime < rule.EndTime;
            }

            // Evening portion starting on the selected date.
            var eveningAvailable =
                rule.Month == selectedDate.Month &&
                selectedTime >= rule.StartTime;

            // Morning portion from a window starting yesterday.
            // This uses the actual previous calendar date,
            // including at month and year boundaries.
            var morningAvailable =
                rule.Month == previousDate.Month &&
                selectedTime < rule.EndTime;

            return eveningAvailable || morningAvailable;
        }

        private static bool WeatherMatches(
            string requiredWeather,
            string selectedWeather)
        {
            // Unknown means potentially catchable.
            if (selectedWeather.Equals(
                "Unknown",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (requiredWeather.Equals(
                "Any",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (requiredWeather.Equals(
                "Rain Only",
                StringComparison.OrdinalIgnoreCase))
            {
                return selectedWeather.Equals(
                    "Rain",
                    StringComparison.OrdinalIgnoreCase);
            }

            if (requiredWeather.Equals(
                "No Rain",
                StringComparison.OrdinalIgnoreCase))
            {
                return !selectedWeather.Equals(
                    "Rain",
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static object BuildStatistics(
            List<DashboardSpecies> species)
        {
            return new
            {
                total = new
                {
                    donated = species.Count(x => x.IsDonated),
                    available = species.Count
                },

                rightNow = new
                {
                    donated = species.Count(x =>
                        x.AvailableNow && x.IsDonated),
                    available = species.Count(x =>
                        x.AvailableNow)
                },

                totalDay = new
                {
                    donated = species.Count(x =>
                        x.AvailableToday && x.IsDonated),
                    available = species.Count(x =>
                        x.AvailableToday)
                }
            };
        }

        private static string FormatTime(TimeSpan value)
        {
            return DateTime.Today
                .Add(value)
                .ToString(
                    "h:mm tt",
                    CultureInfo.InvariantCulture);
        }

        // Existing Fish Tracker.
        [HttpGet("fish")]
        public async Task<IActionResult> Fish()
        {
            var donatedIds = await _context.MuseumDonations
                .AsNoTracking()
                .Where(d => d.TownID == TownID)
                .Select(d => d.MuseumItemID)
                .ToListAsync();

            var donatedSet = donatedIds.ToHashSet();

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

        // Existing fish donation endpoint.
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

            var existingDonation =
                await _context.MuseumDonations
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

        private sealed class AvailabilityRule
        {
            public int MuseumItemID { get; set; }
            public int Month { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public bool IsAllDay { get; set; }

            public string WeatherCondition { get; set; }
                = string.Empty;

            public bool IsIslandOnly { get; set; }
        }

        private sealed class DashboardSpecies
        {
            public int MuseumItemID { get; set; }
            public string Name { get; set; } = string.Empty;
            public int CategoryID { get; set; }
            public int? SellPrice { get; set; }
            public bool IsDonated { get; set; }
            public bool AvailableNow { get; set; }
            public bool AvailableToday { get; set; }
            public bool IsIslandOnly { get; set; }

            public List<string> AvailabilityWindows { get; set; }
                = new();
        }
    }
}
