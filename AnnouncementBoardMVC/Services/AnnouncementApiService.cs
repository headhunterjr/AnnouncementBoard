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
            var categories = await _http.GetFromJsonAsync<IEnumerable<AnnouncementCategory>>("api/categories");
            return categories!.Select(c => c.Category);
        }

        public async Task<IEnumerable<string>> GetSubCategoriesAsync()
        {
            var subcategories = await _http.GetFromJsonAsync<IEnumerable<AnnouncementSubCategory>>("api/subcategories");
            return subcategories!.Select(s => s.SubCategory);
        }

        public async Task<IEnumerable<string>> GetSubCategoriesByCategoryAsync(string category)
        {
            var subcategories = await _http.GetFromJsonAsync<IEnumerable<AnnouncementSubCategory>>($"api/subcategories/{Uri.EscapeDataString(category)}");
            return subcategories!.Select(s => s.SubCategory);
        }

        public async Task<IEnumerable<Announcement>?> FilterAsync(
            IEnumerable<string>? categories,
            IEnumerable<string>? subcategories)
        {
            var formData = new List<KeyValuePair<string, string>>();

            if (categories != null)
            {
                foreach (var c in categories)
                    formData.Add(new KeyValuePair<string, string>("categories", c));
            }

            if (subcategories != null)
            {
                foreach (var s in subcategories)
                    formData.Add(new KeyValuePair<string, string>("subcategories", s));
            }

            using var content = new FormUrlEncodedContent(formData);
            var resp = await _http.PostAsync("api/announcements/filter", content);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<Announcement>>();
        }

        public async Task<Announcement?> RefreshAnnouncementAsync(int id)
        {
            var resp = await _http.PutAsync($"api/announcements/{id}/refresh", null);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<Announcement>();
        }

    }
}
