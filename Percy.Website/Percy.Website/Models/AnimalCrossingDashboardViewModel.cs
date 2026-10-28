
using System;
using System.Collections.Generic;

namespace Percy.Website.Models
{
    public class AnimalCrossingDashboardViewModel
    {
        public string TownName { get; set; } = "Maple";

        public DateOnly SelectedDate { get; set; }
            = DateOnly.FromDateTime(DateTime.Today);

        public TimeOnly SelectedTime { get; set; }
            = TimeOnly.FromDateTime(DateTime.Now);

        public string Weather { get; set; } = "Unknown";

        public bool IsManualDateTime { get; set; }

        public CollectionStatisticsViewModel Fish { get; set; }
            = new();

        public CollectionStatisticsViewModel Insects { get; set; }
            = new();

        public CollectionStatisticsViewModel Fossils { get; set; }
            = new();

        public CollectionStatisticsViewModel Art { get; set; }
            = new();

        public List<CatchableSpeciesViewModel> Species { get; set; }
            = new();

        public List<AnimalCrossingEventViewModel> Events { get; set; }
            = new();
    }

    public class CollectionStatisticsViewModel
    {
        public int TotalDonated { get; set; }

        public int TotalSpecies { get; set; }

        public int AvailableNowDonated { get; set; }

        public int AvailableNowTotal { get; set; }

        public int AvailableTodayDonated { get; set; }

        public int AvailableTodayTotal { get; set; }
    }

    public class CatchableSpeciesViewModel
    {
        public int MuseumItemID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public int? SellPrice { get; set; }

        public string Location { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public bool IsDonated { get; set; }

        public bool AvailableNow { get; set; }

        public bool AvailableToday { get; set; }

        public bool IsIslandOnly { get; set; }

        public List<string> AvailabilityWindows { get; set; }
            = new();

        public string WeatherRequirement { get; set; }
            = "Any";
    }

    public class AnimalCrossingEventViewModel
    {
        public string EventName { get; set; } = string.Empty;

        public DateOnly EventDate { get; set; }

        public string? StartTime { get; set; }

        public string? Description { get; set; }

        public bool IsToday { get; set; }
    }
}
