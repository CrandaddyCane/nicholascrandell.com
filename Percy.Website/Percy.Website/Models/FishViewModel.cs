
namespace Percy.Website.Models
{
    public class FishViewModel
    {
        public int MuseumItemID { get; set; }

        public string FishName { get; set; } = string.Empty;

        public int? SellPrice { get; set; }

        public string SpawnLocations { get; set; } = string.Empty;

        public int? ShadowSize { get; set; }

        public bool IsDonated { get; set; }

        // Months when this fish is available.
        // Values are 1 through 12.
        public List<int> AvailableMonths { get; set; } = new();

        // Availability windows across the year.
        // Each record retains its month and time range.
        public List<FishAvailabilityWindowViewModel>
            AvailabilityWindows
        { get; set; } = new();
    }

    public class FishAvailabilityWindowViewModel
    {
        public int Month { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public string WeatherCondition { get; set; } = string.Empty;
    }
}
