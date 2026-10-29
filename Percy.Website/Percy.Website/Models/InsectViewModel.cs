
namespace Percy.Website.Models
{
    public class InsectViewModel
    {
        public int MuseumItemID { get; set; }

        public string InsectName { get; set; } = string.Empty;

        public int? SellPrice { get; set; }

        public string SpawnLocations { get; set; } = string.Empty;

        public bool IsDonated { get; set; }

        public bool IsIslandOnly { get; set; }

        // Months when the insect can be caught.
        // Values range from 1 through 12.
        public List<int> AvailableMonths { get; set; } = new();

        // Individual monthly and hourly availability records.
        public List<InsectAvailabilityWindowViewModel>
            AvailabilityWindows
        { get; set; } = new();
    }

    public class InsectAvailabilityWindowViewModel
    {
        public int Month { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public string WeatherCondition { get; set; } = string.Empty;

        public bool IsIslandOnly { get; set; }

        public string LocationName { get; set; } = string.Empty;
    }
}
