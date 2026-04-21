using KitapSatis.Api.Data;
using KitapSatis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BooksController(AppDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] string? search, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var query = _db.Books
                .Include(b => b.Category)
                .Where(b => b.IsActive)
                .AsNoTracking();

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(b => b.Name.ToLower().Contains(lowerSearch) || (b.Author != null && b.Author.ToLower().Contains(lowerSearch)));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
            }

            var books = await query.ToListAsync();
            return Ok(books);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _db.Books
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);

            if (book == null) return NotFound();
            return Ok(book);
        }

        public record BookRequest(
            string Name, string? Author, string? Publisher, string? ISBN,
            string? Language, string? Dimensions, int? PublicationYear, int? PageCount,
            string? Description, string? ImageUrl, decimal Price, int StockQuantity,
            int MinStockLevel, decimal ShippingFee, bool IsFeatured, bool IsActive, int? CategoryId
        );

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("Name required");
            if (req.Price < 0) return BadRequest("Price cannot be negative");

            var book = new Book
            {
                Name = req.Name,
                Author = req.Author,
                Publisher = req.Publisher,
                ISBN = req.ISBN,
                Language = req.Language,
                Dimensions = req.Dimensions,
                PublicationYear = req.PublicationYear,
                PageCount = req.PageCount,
                Description = req.Description,
                ImageUrl = req.ImageUrl,
                Price = req.Price,
                StockQuantity = req.StockQuantity,
                MinStockLevel = req.MinStockLevel,
                ShippingFee = req.ShippingFee,
                IsFeatured = req.IsFeatured,
                IsActive = req.IsActive,
                CategoryId = req.CategoryId
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            
            // Reload the category to return it in the response
            book.Category = await _db.Categories.FindAsync(book.CategoryId);
            
            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookRequest req)
        {
            var existing = await _db.Books.FindAsync(id);
            if (existing == null) return NotFound();

            if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("Name required");
            if (req.Price < 0) return BadRequest("Price cannot be negative");

            existing.Name = req.Name;
            existing.Author = req.Author;
            existing.Publisher = req.Publisher;
            existing.ISBN = req.ISBN;
            existing.Language = req.Language;
            existing.Dimensions = req.Dimensions;
            existing.PublicationYear = req.PublicationYear;
            existing.PageCount = req.PageCount;
            existing.Description = req.Description;
            existing.ImageUrl = req.ImageUrl;
            existing.Price = req.Price;
            existing.StockQuantity = req.StockQuantity;
            existing.MinStockLevel = req.MinStockLevel;
            existing.ShippingFee = req.ShippingFee;
            existing.IsFeatured = req.IsFeatured;
            existing.IsActive = req.IsActive;
            existing.CategoryId = req.CategoryId;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.IsActive = false; // Soft delete
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
