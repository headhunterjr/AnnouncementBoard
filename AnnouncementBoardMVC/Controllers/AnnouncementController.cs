using AnnouncementBoardMVC.Models;
using AnnouncementBoardMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AnnouncementBoardMVC.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IAnnouncementApiService _api;

        public AnnouncementController(IAnnouncementApiService api)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var items = await _api.GetAllAsync();
                return View(items);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load announcements. Please try again later.";
                return View(Enumerable.Empty<Announcement>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var item = await _api.GetByIdAsync(id);
                return item == null ? NotFound() : View(item);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load announcement details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = await _api.GetCategoriesAsync();
                ViewBag.SubCategories = Enumerable.Empty<string>();
                return View(new Announcement());
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load categories. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Announcement model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _api.GetCategoriesAsync();
                    ViewBag.SubCategories = Enumerable.Empty<string>();
                    return View(model);
                }

                await _api.CreateAsync(model);
                TempData["SuccessMessage"] = "Announcement created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                TempData["ErrorMessage"] = "Invalid input data. Please check your form and try again.";

                ViewBag.Categories = await _api.GetCategoriesAsync();
                ViewBag.SubCategories = !string.IsNullOrEmpty(model.Category) ? await _api.GetSubCategoriesByCategoryAsync(model.Category) : Enumerable.Empty<string>();
                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to create announcement. Please try again later.";

                ViewBag.Categories = await _api.GetCategoriesAsync();
                ViewBag.SubCategories = !string.IsNullOrEmpty(model.Category) ? await _api.GetSubCategoriesByCategoryAsync(model.Category) : Enumerable.Empty<string>();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var announcement = await _api.GetByIdAsync(id);
                if (announcement == null) return NotFound();
                ViewBag.Categories = await _api.GetCategoriesAsync();
                ViewBag.SubCategories = await _api.GetSubCategoriesByCategoryAsync(announcement.Category);
                return View(announcement);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load announcement for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Announcement model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _api.GetCategoriesAsync();
                    ViewBag.SubCategories = await _api.GetSubCategoriesByCategoryAsync(model.Category);
                    return View(model);
                }

                model.Id = id;
                await _api.UpdateAsync(model);
                TempData["SuccessMessage"] = "Announcement updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                TempData["ErrorMessage"] = "Invalid input data. Please check your form and try again.";

                ViewBag.Categories = await _api.GetCategoriesAsync();
                ViewBag.SubCategories = !string.IsNullOrEmpty(model.Category) ? await _api.GetSubCategoriesByCategoryAsync(model.Category) : Enumerable.Empty<string>();

                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to update announcement. Please try again later.";

                ViewBag.Categories = await _api.GetCategoriesAsync();
                ViewBag.SubCategories = !string.IsNullOrEmpty(model.Category) ? await _api.GetSubCategoriesByCategoryAsync(model.Category) : Enumerable.Empty<string>();

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _api.DeleteAsync(id);
                TempData["SuccessMessage"] = "Announcement deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to delete announcement. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter()
        {
            var vm = new AnnouncementFilterViewModel
            {
                AllCategories = (await _api.GetCategoriesAsync()).ToList()
            };

            foreach (var cat in vm.AllCategories)
            {
                var subs = await _api.GetSubCategoriesByCategoryAsync(cat);
                vm.AllSubCategories[cat] = subs.ToList();
            }

            vm.SelectedCategories = vm.AllCategories.ToList();
            vm.SelectedSubCategories = vm.AllSubCategories.Values.SelectMany(x => x).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter([FromForm] string[]? SelectedCategories, [FromForm] string[]? SelectedSubCategories, AnnouncementFilterViewModel vm)
        {
            vm.AllCategories = (await _api.GetCategoriesAsync()).ToList();
            vm.AllSubCategories = new Dictionary<string, List<string>>();
            vm.SelectedCategories = SelectedCategories?.ToList() ?? new();
            vm.SelectedSubCategories = SelectedSubCategories?.ToList() ?? new();
            vm.Results = (await _api.FilterAsync(vm.SelectedCategories, vm.SelectedSubCategories)).ToList();

            return View(vm);
        }
    }
}
