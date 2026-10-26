
namespace Percy.Website.Models
{
    public class ExperienceViewModel
    {
        public string Company { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public string? WebsiteUrl { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string DateRange { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<string> Technologies { get; set; } = new();
    }
}
