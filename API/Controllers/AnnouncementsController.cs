using AnnouncementBoard.Data;
using AnnouncementBoard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AnnouncementBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementRepository _repository;

        public AnnouncementsController(IAnnouncementRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var announcement = await _repository.GetByIdAsync(id);
            return announcement is null ? NotFound() : Ok(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Announcement announcement)
        {
            try
            {
                var newId = await _repository.CreateAsync(announcement);
                var added = await _repository.GetByIdAsync(newId);
                return CreatedAtAction(nameof(Get), new { id = newId }, added);
            }
            catch (SqlException)
            {
                return BadRequest("Invalid input data.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Announcement announcement)
        {
            if (id != announcement.Id)
            {
                return BadRequest("Invalid ID: the body should match the URL");
            }
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }
            try
            {
                await _repository.UpdateAsync(announcement);
                return NoContent();
            }
            catch (SqlException)
            {
                return BadRequest("Invalid input data.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromForm] string[]? categories, [FromForm] string[]? subcategories)
        {
            var results = await _repository.FilterAsync(categories, subcategories);
            return Ok(results);
        }

        [HttpPut("{id}/refresh")]
        public async Task<IActionResult> RefreshDate(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            await _repository.RefreshAnnouncementAsync(id);

            var updated = await _repository.GetByIdAsync(id);
            return Ok(updated);
        }
    }
}
