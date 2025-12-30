using KitapSatis.Api.Models;

namespace KitapSatis.Api.Services
{
    public interface IOrderService
    {
        Task<Order> CreateFromBookAsync(int userId, int bookId, int quantity);

        
        Task<List<Order>> GetMineAsync(int userId);
        Task<List<Order>> GetAllAsync();

        
        Task<Order> MarkPaidAsync(int orderId);
        Task<Order> CancelAsync(int orderId, int userId, bool isAdmin);
    }
}
