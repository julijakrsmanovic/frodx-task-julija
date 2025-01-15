using Order_Processing_Worker_Service.EFCore.Models;

public interface IDbOrderService
{
    Task AddOrdersAsync(List<Order> orders);
}