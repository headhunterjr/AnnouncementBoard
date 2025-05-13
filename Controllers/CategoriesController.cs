using AnnouncementBoard.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnnouncementBoard.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IAnnouncementRepository _repository;

        public CategoriesController(IAnnouncementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var cats = await _repository.GetCategoriesAsync();
            return Ok(cats);
        }

        [HttpGet("subcategories")]
        public async Task<IActionResult> GetSubCategories()
        {
            var subcategories = await _repository.GetSubCategoriesAsync();
            return Ok(subcategories);
        }

        [HttpGet("subcategories/{category}")]
        public async Task<IActionResult> GetSubCategoriesByCategory(string category)
        {
            var subcategories = await _repository.GetSubCategoriesByCategoryAsync(category);
            if (!subcategories.Any())
            {
                return NotFound($"No subcategories found for category '{category}'.");
            }
            return Ok(subcategories);
        }
    }
}
