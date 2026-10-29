
namespace Percy.Website.Models
{
    public class FossilCollectionViewModel
    {
        public string TownName { get; set; } = string.Empty;

        public string PlayerName { get; set; } = string.Empty;

        public List<FossilGroupViewModel> Groups { get; set; }
            = new();

        public int TotalFossils =>
            Groups.Sum(group => group.Fossils.Count);

        public int DonatedFossils =>
            Groups.Sum(group =>
                group.Fossils.Count(fossil => fossil.IsDonated));

        public int CompletedGroups =>
            Groups.Count(group =>
                group.Fossils.Count > 0 &&
                group.Fossils.All(fossil => fossil.IsDonated));
    }

    public class FossilGroupViewModel
    {
        public int FossilGroupID { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public List<FossilItemViewModel> Fossils { get; set; }
            = new();

        public int TotalFossils => Fossils.Count;

        public int DonatedFossils =>
            Fossils.Count(fossil => fossil.IsDonated);

        public bool IsComplete =>
            Fossils.Count > 0 &&
            Fossils.All(fossil => fossil.IsDonated);
    }

    public class FossilItemViewModel
    {
        public int MuseumItemID { get; set; }

        public string FossilName { get; set; } = string.Empty;

        public bool IsDonated { get; set; }
    }
}
