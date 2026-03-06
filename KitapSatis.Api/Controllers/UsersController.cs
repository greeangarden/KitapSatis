using KitapSatis.Api.Data;
using KitapSatis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KitapSatis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        // Admin: Get All Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? role, [FromQuery] string? search)
        {
            var query = _db.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(u => 
                    u.Email!.ToLower().Contains(lowerSearch) || 
                    u.StudentNumber!.Contains(search) ||
                    (u.FirstName + " " + u.LastName).ToLower().Contains(lowerSearch));
            }

            var users = await query.ToListAsync();
            return Ok(users);
        }

        // Admin: Get User details with Orders
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.CreatedAtUtc)
                .AsNoTracking()
                .ToListAsync();

            return Ok(new { User = user, Orders = orders });
        }

        // Admin: Update User
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] User updateReq)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = updateReq.FirstName;
            user.LastName = updateReq.LastName;
            user.Email = updateReq.Email;
            user.PhoneNumber = updateReq.PhoneNumber;
            user.StudentNumber = updateReq.StudentNumber;
            user.Department = updateReq.Department;
            user.Role = updateReq.Role;
            user.IsActive = updateReq.IsActive;

            await _db.SaveChangesAsync();
            return Ok(user);
        }

        // User/Admin: Get Own Profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue("uid")!);
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();
            return Ok(user);
        }

        public record UpdateProfileRequest(string? FirstName, string? LastName, string? Email, string? PhoneNumber, string? Department, string? StudentNumber);

        // User: Update Own Profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
        {
            var userId = int.Parse(User.FindFirstValue("uid")!);
            var user = await _db.Users.FindAsync(userId);

            if (user == null) return NotFound();

            user.FirstName = req.FirstName;
            user.LastName = req.LastName;
            user.Email = req.Email;
            user.PhoneNumber = req.PhoneNumber;
            user.Department = req.Department;
            user.StudentNumber = req.StudentNumber;

            await _db.SaveChangesAsync();
            return Ok(user);
        }
    }
}
