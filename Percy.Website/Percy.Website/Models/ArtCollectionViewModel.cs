
namespace Percy.Website.Models
{
    public class ArtCollectionViewModel
    {
        public string TownName { get; set; } = string.Empty;

        public string PlayerName { get; set; } = string.Empty;

        public List<ArtItemViewModel> Paintings { get; set; }
            = new();

        public int TotalPaintings => Paintings.Count;

        public int DonatedPaintings =>
            Paintings.Count(painting => painting.IsDonated);

        public double CompletionPercentage =>
            TotalPaintings > 0
                ? (double)DonatedPaintings / TotalPaintings * 100
                : 0;
    }

    public class ArtItemViewModel
    {
        public int MuseumItemID { get; set; }

        public string PaintingName { get; set; } = string.Empty;

        public bool IsDonated { get; set; }
    }
}
