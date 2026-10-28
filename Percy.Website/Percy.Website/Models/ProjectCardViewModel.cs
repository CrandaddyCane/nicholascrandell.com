
namespace Percy.Website.Models
{
    public class ProjectCardViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string ButtonText { get; set; } = "Open";

        public string? Url { get; set; }

        public bool IsEnabled { get; set; } = true;

        public string? Status { get; set; }
    }
}
