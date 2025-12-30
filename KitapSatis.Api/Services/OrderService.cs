using KitapSatis.Api.Data;
using KitapSatis.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ NEW: BookId ile sipariş oluşturma (fiyat DB’den)
        public async Task<Order> CreateFromBookAsync(int userId, int bookId, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be > 0");

            var book = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookId && b.IsActive);

            if (book == null)
                throw new KeyNotFoundException("Book not found");

            var order = new Order
            {
                UserId = userId,
                BookId = book.Id,
                Quantity = quantity,
                UnitPrice = book.Price,
                TotalPrice = book.Price * quantity,

                Status = OrderStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // ✅ User kendi siparişleri
        public async Task<List<Order>> GetMineAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAtUtc)
                .ToListAsync();
        }

        // ✅ Admin tüm siparişler
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAtUtc)
                .ToListAsync();
        }

        // ✅ Admin ödenmiş yapar
        public async Task<Order> MarkPaidAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cancelled order cannot be paid");

            order.Status = OrderStatus.Paid;
            await _context.SaveChangesAsync();
            return order;
        }

        // ✅ İptal (admin herkesinkini, user sadece kendininkini)
        public async Task<Order> CancelAsync(int orderId, int userId, bool isAdmin)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");

            if (!isAdmin && order.UserId != userId)
                throw new UnauthorizedAccessException("You can cancel only your own order");

            if (order.Status == OrderStatus.Paid)
                throw new InvalidOperationException("Paid order cannot be cancelled");

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
