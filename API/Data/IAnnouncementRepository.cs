using AnnouncementBoard.Models;

namespace AnnouncementBoard.Data
{
    public interface IAnnouncementRepository
    {
        public Task<IEnumerable<Announcement>> GetAllAsync();
        public Task<Announcement?> GetByIdAsync(int id);
        public Task<int> CreateAsync(Announcement a);
        public Task UpdateAsync(Announcement a);
        public Task DeleteAsync(int id);
        public Task<IEnumerable<AnnouncementCategory>> GetCategoriesAsync();
        public Task<IEnumerable<AnnouncementSubCategory>> GetSubCategoriesAsync();
        public Task<IEnumerable<AnnouncementSubCategory>> GetSubCategoriesByCategoryAsync(string category);
        public Task<IEnumerable<Announcement>> FilterAsync(IEnumerable<string>? categories, IEnumerable<string>? subcategories);
        public Task RefreshAnnouncementAsync(int id);
    }
}
