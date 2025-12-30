using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KitapSatis.Api.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    int CurrentUserId() => int.Parse(User.FindFirstValue("uid")!);
    bool IsAdmin() => User.IsInRole("Admin");

    public record CreateOrderRequest(int BookId, int Quantity);

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
    {
        var created = await _orderService.CreateFromBookAsync(
            CurrentUserId(),
            req.BookId,
            req.Quantity
        );

        return Ok(created);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var list = await _orderService.GetMineAsync(CurrentUserId());
        return Ok(list);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> All()
    {
        var list = await _orderService.GetAllAsync();
        return Ok(list);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var cancelled = await _orderService.CancelAsync(id, CurrentUserId(), IsAdmin());
        return Ok(cancelled);
    }
}
