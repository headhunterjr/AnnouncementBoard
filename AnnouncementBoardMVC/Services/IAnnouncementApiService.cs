using AnnouncementBoardMVC.Models;

namespace AnnouncementBoardMVC.Services
{
    public interface IAnnouncementApiService
    {
        Task<IEnumerable<Announcement>> GetAllAsync();
        Task<Announcement?> GetByIdAsync(int id);
        Task<int> CreateAsync(Announcement a);
        Task UpdateAsync(Announcement a);
        Task DeleteAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<string>> GetSubCategoriesAsync();
        Task<IEnumerable<string>> GetSubCategoriesByCategoryAsync(string category);
    }
}
