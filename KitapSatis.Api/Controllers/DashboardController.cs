using KitapSatis.Api.Data;
using KitapSatis.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalRevenue = await _db.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .SumAsync(o => o.TotalPrice);

            var totalOrders = await _db.Orders.CountAsync();

            var booksSold = await _db.OrderItems
                .Where(oi => oi.Order != null && oi.Order.Status == OrderStatus.Paid)
                .SumAsync(oi => oi.Quantity);

            var totalUsers = await _db.Users
                .Where(u => u.Role != "Admin")
                .CountAsync();

            var recentOrders = await _db.Orders
                .OrderByDescending(o => o.CreatedAtUtc)
                .Take(5)
                .Select(o => new {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAtUtc
                })
                .ToListAsync();

            var topSellingBooks = await _db.OrderItems
                .Where(oi => oi.Order != null && oi.Order.Status == OrderStatus.Paid)
                .GroupBy(oi => oi.BookId)
                .Select(g => new {
                    BookId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                BooksSold = booksSold,
                TotalUsers = totalUsers,
                RecentOrders = recentOrders,
                TopSellingBooks = topSellingBooks
            });
        }
    }
}
