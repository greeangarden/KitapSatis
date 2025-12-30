using KitapSatis.Api.Data;
using KitapSatis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _db;
    public BooksController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await _db.Books
            .AsNoTracking()
            .Where(b => b.IsActive)
            .ToListAsync();

        return Ok(books);
    }

   
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Name)) return BadRequest("Name required");
        if (book.Price <= 0) return BadRequest("Price must be > 0");

        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return Ok(book);
    }
}
