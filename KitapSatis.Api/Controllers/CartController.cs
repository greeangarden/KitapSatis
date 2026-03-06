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
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CartController(AppDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId() => int.Parse(User.FindFirstValue("uid")!);

        private async Task<Order> GetOrCreateCartAsync()
        {
            var userId = CurrentUserId();
            var cart = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == OrderStatus.Pending);

            if (cart == null)
            {
                cart = new Order
                {
                    UserId = userId,
                    OrderNumber = "#CRT-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"),
                    Status = OrderStatus.Pending,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _db.Orders.Add(cart);
                await _db.SaveChangesAsync();
            }

            return cart;
        }

        private void RecalculateTotal(Order cart)
        {
            var itemsTotal = cart.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
            
            if (cart.OrderItems.Any())
            {
                cart.ShippingFee = cart.OrderItems.Max(oi => oi.Book?.ShippingFee ?? 0);
            }
            else
            {
                cart.ShippingFee = 0;
            }

            cart.TotalPrice = itemsTotal + cart.ShippingFee;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await GetOrCreateCartAsync();
            return Ok(cart);
        }

        public record AddToCartRequest(int BookId, int Quantity);

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest req)
        {
            if (req.Quantity <= 0) return BadRequest("Miktar sıfırdan büyük olmalıdır.");

            var book = await _db.Books.FindAsync(req.BookId);
            if (book == null || !book.IsActive) return NotFound("Kitap bulunamadı veya satışa kapalı.");

            var cart = await GetOrCreateCartAsync();

            var existingItem = cart.OrderItems.FirstOrDefault(oi => oi.BookId == req.BookId);
            if (existingItem != null)
            {
                existingItem.Quantity += req.Quantity;
                existingItem.UnitPrice = book.Price; // Update to latest price
            }
            else
            {
                cart.OrderItems.Add(new OrderItem
                {
                    BookId = book.Id,
                    Book = book,
                    Quantity = req.Quantity,
                    UnitPrice = book.Price
                });
            }

            RecalculateTotal(cart);
            await _db.SaveChangesAsync();

            return Ok(cart);
        }

        [HttpPut("items/{bookId}")]
        public async Task<IActionResult> UpdateItemQuantity(int bookId, [FromBody] int quantity)
        {
            var cart = await GetOrCreateCartAsync();
            var item = cart.OrderItems.FirstOrDefault(oi => oi.BookId == bookId);

            if (item == null) return NotFound("Ürün sepette değil.");

            if (quantity <= 0)
            {
                cart.OrderItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            RecalculateTotal(cart);
            await _db.SaveChangesAsync();

            return Ok(cart);
        }

        [HttpDelete("items/{bookId}")]
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cart = await GetOrCreateCartAsync();
            var item = cart.OrderItems.FirstOrDefault(oi => oi.BookId == bookId);

            if (item != null)
            {
                cart.OrderItems.Remove(item);
                RecalculateTotal(cart);
                await _db.SaveChangesAsync();
            }

            return Ok(cart);
        }

        public record CheckoutRequest(string DeliveryAddress);

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            var cart = await GetOrCreateCartAsync();
            if (!cart.OrderItems.Any())
                return BadRequest("Sepetiniz boş.");

            // Normally interact with payment gateway here (Iyzico etc.)

            cart.DeliveryAddress = req.DeliveryAddress;
            // ShippingFee and TotalPrice are already calculated properly by the backend

            cart.Status = OrderStatus.Paid; // Assume mocked successfully paid for now
            cart.OrderNumber = cart.OrderNumber.Replace("CRT", "ORD"); // Change prefix to Order

            // Check and decrease stock
            foreach (var item in cart.OrderItems)
            {
                var book = await _db.Books.FindAsync(item.BookId);
                if (book != null)
                {
                    if (book.StockQuantity < item.Quantity)
                        return BadRequest($"'{book.Name}' stokta yeterli değil. Sadece {book.StockQuantity} adet var.");

                    book.StockQuantity -= item.Quantity;
                    // book.MinStockLevel can be used here later to trigger low-stock alerts to admins
                }
                else
                {
                    return BadRequest("Sepetinizdeki bazı ürünler sistemden kaldırılmış.");
                }
            }

            await _db.SaveChangesAsync();
            return Ok(cart);
        }
    }
}
