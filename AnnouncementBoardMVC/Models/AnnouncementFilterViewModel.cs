namespace AnnouncementBoardMVC.Models
{
    public class AnnouncementFilterViewModel
    {
        public List<string> AllCategories { get; set; } = new();
        public Dictionary<string, List<string>> AllSubCategories { get; set; } = new();
        public List<string> SelectedCategories { get; set; } = new();
        public List<string> SelectedSubCategories { get; set; } = new();
        public List<Announcement> Results { get; set; } = new();
    }
}
