using AnnouncementBoard.Data;
using AnnouncementBoard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var newId = await _repository.CreateAsync(announcement);
            return CreatedAtAction(nameof(Get), new { id = newId }, announcement);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Announcement announcement)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
            {
                return NotFound();
            }
            if (id != announcement.Id)
            {
                return BadRequest();
            }
            await _repository.UpdateAsync(announcement);
            return NoContent();
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
    }
}
