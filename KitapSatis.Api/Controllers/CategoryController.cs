using KitapSatis.Api.Data;
using KitapSatis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _db.Categories
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync();
            return Ok(categories);
        }

        public record CreateCategoryRequest(string Name, bool IsActive = true);

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return BadRequest("Category name is required.");

            var category = new Category { Name = req.Name, IsActive = req.IsActive };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return Ok(category);
        }

        public record UpdateCategoryRequest(string Name, bool IsActive);

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest req)
        {
            var existing = await _db.Categories.FindAsync(id);
            if (existing == null) return NotFound();

            if (string.IsNullOrWhiteSpace(req.Name))
                return BadRequest("Category name is required.");

            existing.Name = req.Name;
            existing.IsActive = req.IsActive;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.IsActive = false; // Soft delete
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
