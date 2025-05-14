using AnnouncementBoardMVC.Models;

namespace AnnouncementBoardMVC.Services
{
    public class AnnouncementApiService : IAnnouncementApiService
    {
        private readonly HttpClient _http;
        public AnnouncementApiService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("AnnouncementBoardAPI");
        }

        public Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<Announcement>>("api/announcements")!;
        }

        public Task<Announcement?> GetByIdAsync(int id)
        {
            return _http.GetFromJsonAsync<Announcement>($"api/announcements/{id}");
        }

        public async Task<int> CreateAsync(Announcement announcement)
        {
            var response = await _http.PostAsJsonAsync("api/announcements", announcement);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Announcement>();
            return created!.Id;
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            var response = await _http.PutAsJsonAsync($"api/announcements/{announcement.Id}", announcement);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/announcements/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var categories = await _http.GetFromJsonAsync<IEnumerable<AnnouncementCategory>>("api/lookups/categories");
            return categories!.Select(c => c.Category);
        }

        public async Task<IEnumerable<string>> GetSubCategoriesAsync()
        {
            var subcategories = await _http.GetFromJsonAsync<IEnumerable<AnnouncementSubCategory>>("api/lookups/subcategories");
            return subcategories!.Select(s => s.SubCategory);
        }

        public async Task<IEnumerable<string>> GetSubCategoriesByCategoryAsync(string category)
        {
            var subcategories = await _http.GetFromJsonAsync<IEnumerable<AnnouncementSubCategory>>($"api/lookups/subcategories/{Uri.EscapeDataString(category)}");
            return subcategories!.Select(s => s.SubCategory);
        }
    }
}
