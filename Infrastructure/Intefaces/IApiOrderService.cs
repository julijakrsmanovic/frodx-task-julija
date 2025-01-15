using Order_Processing_Worker_Service.EFCore.Models;

public interface IApiOrderService
{
    Task<List<Order>> FetchOrdersFromApiAsync();
}
