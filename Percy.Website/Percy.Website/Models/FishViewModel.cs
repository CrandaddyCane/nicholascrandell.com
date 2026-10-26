
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
    }
}
