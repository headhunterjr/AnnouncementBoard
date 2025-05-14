using AnnouncementBoardMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnnouncementBoardMVC.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IAnnouncementApiService _api;

        public AnnouncementController(IAnnouncementApiService api)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
        }

        public async Task<IActionResult> Index()
        {
            var items = await _api.GetAllAsync();
            return View(items);
        }
    }
}
