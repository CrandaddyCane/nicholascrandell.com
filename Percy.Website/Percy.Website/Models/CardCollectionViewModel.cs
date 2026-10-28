
using System.Collections.Generic;

namespace Percy.Website.Models
{
    public class CardCollectionViewModel
    {
        public string CollectionId { get; set; } = string.Empty;

        public List<ProjectCardViewModel> Cards { get; set; } = new();

        public bool ShowNavigation { get; set; } = true;

        // Supported variants:
        // "horizontal" - Wide application cards.
        // "compact" - Smaller image-based cards.
        public string DisplayVariant { get; set; } = "compact";
    }
}
