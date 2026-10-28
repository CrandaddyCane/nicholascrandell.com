
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

        private const string SpriteDirectory =
            "/images/animal-crossing/sprites/";

        // Original GameCube and Doubutsu no Mori e+ fish sprites.
        private static readonly IReadOnlyDictionary<int, string>
            FishSpriteFiles = new Dictionary<int, string>
            {
                [1] = "Angelfish_PG_Icon.png",
                [2] = "Arapaima_PG_Icon.png",
                [3] = "Arowana_PG_Icon.png",
                [4] = "Barbel_Steed_PG_Icon.png",
                [5] = "Barred_Knifejaw_PG_Icon.png",
                [6] = "Bass_PG_Inv_Icon.png",
                [7] = "Bitterling_PG_Icon.png",
                [8] = "Blue_Marlin_DnMe+_Icon.png",
                [9] = "Bluegill_PG_Icon.png",
                [10] = "Brook_Trout_PG_Icon.png",
                [11] = "Carp_PG_Icon.png",
                [12] = "Catfish_PG_Icon.png",
                [13] = "Cherry_Salmon_PG_Icon.png",
                [14] = "Coelacanth_PG_Icon.png",
                [15] = "Crawfish_PG_Icon.png",
                [16] = "Crucian_Carp_PG_Icon.png",
                [17] = "Dab_DnMe+_Icon.png",
                [18] = "Dace_PG_Icon.png",
                [19] = "Eel_PG_Icon.png",
                [20] = "Freshwater_Goby_PG_Icon.png",
                [21] = "Frog_PG_Icon.png",
                [22] = "Giant_Catfish_PG_Icon.png",
                [23] = "Giant_Snakehead_PG_Icon.png",
                [24] = "Goldfish_PG_Icon.png",
                [25] = "Guppy_PG_Icon.png",
                [26] = "Horse_Mackerel_DnMe+_Icon.png",
                [27] = "Jellyfish_PG_Icon.png",
                [28] = "Killifish_PG_Icon.png",
                [29] = "Koi_PG_Icon.png",
                [30] = "Large_Bass_PG_Icon.png",
                [31] = "Large_Char_PG_Icon.png",
                [32] = "Loach_PG_Icon.png",
                [33] = "Octopus_DnMe+_Icon.png",
                [34] = "Olive_Flounder_DnMe+_Icon.png",
                [35] = "Pale_Chub_PG_Icon.png",
                [36] = "Piranha_PG_Icon.png",
                [37] = "Pond_Smelt_PG_Icon.png",
                [38] = "Popeyed_Goldfish_PG_Icon.png",
                [39] = "Balloonfish_DnMe+_Icon.png",
                [40] = "Rainbow_Trout_PG_Icon.png",
                [41] = "Red_Snapper_PG_Icon.png",
                [42] = "Salmon_PG_Icon.png",
                [43] = "Sea_Bass_PG_Icon.png",
                [44] = "Seahorse_DnMe+_Icon.png",
                [45] = "Small_Bass_PG_Icon.png",
                [46] = "Squid_DnMe+_Icon.png",
                [47] = "Stringfish_PG_Icon.png",
                [48] = "Sweetfish_PG_Icon.png"
            };

        // Original GameCube and Doubutsu no Mori e+ insect sprites.
        private static readonly IReadOnlyDictionary<int, string>
            InsectSpriteFiles = new Dictionary<int, string>
            {
                [119] = "Alexandra's_Swallowtail_Butterfly_DnMe+_Icon.png",
                [120] = "Ant_PG_Icon.png",
                [121] = "Bagworm_PG_Icon.png",
                [122] = "Banded_Dragonfly_PG_Icon.png",
                [123] = "Bee_PG_Icon.png",
                [124] = "Bell_Cricket_PG_Icon.png",
                [125] = "Brown_Cicada_PG_Icon.png",
                [126] = "Cockroach_PG_Icon.png",
                [127] = "Coconut_Crab_DnMe+_Icon.png",
                [128] = "Common_Butterfly_PG_Icon.png",
                [129] = "Common_Dragonfly_PG_Icon.png",
                [130] = "Crab_DnMe+_Icon.png",
                [131] = "Cricket_PG_Icon.png",
                [132] = "Darner_Dragonfly_PG_Icon.png",
                [133] = "Diving_Beetle_DnMe+_Icon.png",
                [134] = "Drone_Beetle_PG_Icon.png",
                [135] = "Dung_Beetle_DnMe+_Icon.png",
                [136] = "Dynastid_Beetle_PG_Icon.png",
                [137] = "Evening_Cicada_PG_Icon.png",
                [138] = "Firefly_PG_Icon.png",
                [139] = "Flat_Stag_Beetle_PG_Icon.png",
                [140] = "Flea_DnMe+_Icon.png",
                [141] = "Giant_Beetle_PG_Icon.png",
                [142] = "Grasshopper_PG_Icon.png",
                [143] = "Hercules_Beetle_DnMe+_Icon.png",
                [144] = "Hermit_Crab_DnMe+_Icon.png",
                [145] = "Jewel_Beetle_PG_Icon.png",
                [146] = "Ladybug_PG_Icon.png",
                [147] = "Long_Locust_PG_Icon.png",
                [148] = "Longhorn_Beetle_PG_Icon.png",
                [149] = "Mantis_PG_Icon.png",
                [150] = "Migratory_Locust_PG_Icon.png",
                [151] = "Mole_Cricket_PG_Icon.png",
                [152] = "Mosquito_PG_Icon.png",
                [153] = "Mountain_Beetle_PG_Icon.png",
                [154] = "Pill_Bug_PG_Icon.png",
                [155] = "Pine_Cricket_PG_Icon.png",
                [156] = "Pond_Skater_PG_Icon.png",
                [157] = "Purple_Butterfly_PG_Icon.png",
                [158] = "Red_Dragonfly_PG_Icon.png",
                [159] = "Robust_Cicada_PG_Icon.png",
                [160] = "Saw_Stag_Beetle_PG_Icon.png",
                [161] = "Snail_PG_Icon.png",
                [162] = "Spider_PG_Icon.png",
                [163] = "Spotted_Ladybug_PG_Icon.png",
                [164] = "Tiger_Butterfly_PG_Icon.png",
                [165] = "Walker_Cicada_PG_Icon.png",
                [166] = "Yellow_Butterfly_PG_Icon.png"
            };

        public AnimalCrossingController(
            PercyWorkspaceContext context)
        {
            _context = context;
        }

        // Render the visual dashboard.
        [HttpGet("")]
        public IActionResult Dashboard()
        {
            var model = new AnimalCrossingDashboardViewModel
            {
                TownName = "Maple",
                SelectedDate = DateOnly.FromDateTime(DateTime.Now),
                SelectedTime = TimeOnly.FromDateTime(DateTime.Now),
                Weather = "Unknown",
                IsManualDateTime = false
            };

            return View("Dashboard", model);
        }

        // Read-only dashboard data endpoint.
        [HttpGet("data")]
        public async Task<IActionResult> DashboardData(
            string? date,
            string? time,
            string? weather)
        {
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
                "Unknown", "Sunny", "Cloudy", "Rain", "Snow"
            };

            var requestedWeather = (weather ?? "Unknown").Trim();

            var selectedWeather = validWeather.FirstOrDefault(x =>
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
                .Where(x => x.GameID == GameID && x.IsActive)
                .ToListAsync();

            var donatedIds = (await _context.MuseumDonations
                .AsNoTracking()
                .Where(x => x.TownID == TownID)
                .Select(x => x.MuseumItemID)
                .ToListAsync())
                .ToHashSet();

            var currentMonth = selectedDate.Month;
            var previousMonth = previousDate.Month;

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
                items.Where(x => x.CategoryID == FishCategoryID),
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
                items.Where(x => x.CategoryID == InsectCategoryID),
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

                species = new { fish, insects }
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
                .ToDictionary(x => x.Key, x => x.ToList());

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

                var availableToday = eligibleRules.Any(x =>
                    IsAvailableDuringDate(x, selectedDate));

                var availableNow = eligibleRules.Any(x =>
                    IsAvailableAtTime(
                        x,
                        selectedDate,
                        selectedTime));

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

                string imagePath = string.Empty;

                if (item.CategoryID == FishCategoryID &&
                    FishSpriteFiles.TryGetValue(
                        item.MuseumItemID,
                        out var fishSprite))
                {
                    imagePath = SpriteDirectory + fishSprite;
                }
                else if (item.CategoryID == InsectCategoryID &&
                    InsectSpriteFiles.TryGetValue(
                        item.MuseumItemID,
                        out var insectSprite))
                {
                    imagePath = SpriteDirectory + insectSprite;
                }

                result.Add(new DashboardSpecies
                {
                    MuseumItemID = item.MuseumItemID,
                    Name = item.ItemName,
                    CategoryID = item.CategoryID,
                    SellPrice = item.SellPrice,
                    ImagePath = imagePath,

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

            return result
                .OrderBy(x => x.IsDonated)
                .ThenBy(x => x.Name)
                .ToList();
        }

        private static bool IsOvernight(AvailabilityRule rule)
        {
            return !rule.IsAllDay &&
                   rule.StartTime > rule.EndTime;
        }

        private static bool IsAvailableDuringDate(
            AvailabilityRule rule,
            DateOnly selectedDate)
        {
            var previousDate = selectedDate.AddDays(-1);

            if (rule.Month == selectedDate.Month)
            {
                if (rule.IsAllDay)
                {
                    return true;
                }

                if (!IsOvernight(rule))
                {
                    return rule.EndTime > rule.StartTime;
                }

                return true;
            }

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

            if (rule.IsAllDay)
            {
                return rule.Month == selectedDate.Month;
            }

            if (!IsOvernight(rule))
            {
                return rule.Month == selectedDate.Month &&
                       selectedTime >= rule.StartTime &&
                       selectedTime < rule.EndTime;
            }

            var eveningAvailable =
                rule.Month == selectedDate.Month &&
                selectedTime >= rule.StartTime;

            var morningAvailable =
                rule.Month == previousDate.Month &&
                selectedTime < rule.EndTime;

            return eveningAvailable || morningAvailable;
        }

        private static bool WeatherMatches(
            string requiredWeather,
            string selectedWeather)
        {
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

            public string ImagePath { get; set; }
                = string.Empty;

            public bool IsDonated { get; set; }
            public bool AvailableNow { get; set; }
            public bool AvailableToday { get; set; }
            public bool IsIslandOnly { get; set; }

            public List<string> AvailabilityWindows { get; set; }
                = new();
        }
    }
}
