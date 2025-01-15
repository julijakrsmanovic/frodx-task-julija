using Microsoft.Extensions.Logging;
using Order_Processing_Worker_Service.EFCore.Models;
using System.Text.Json;

public class ApiOrderService : IApiOrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiOrderService> _logger;

    public ApiOrderService(HttpClient httpClient, ILogger<ApiOrderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Order>> FetchOrdersFromApiAsync()
    {
        const string apiUrl = "https://jsonplaceholder.typicode.com/posts";
        try
        {
            _logger.LogInformation("Fetching orders from external API: {ApiUrl}", apiUrl);

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch orders. Status code: {StatusCode}", response.StatusCode);
                return new List<Order>();
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var apiOrders = JsonSerializer.Deserialize<List<ApiOrder>>(responseContent);

            return apiOrders?.Select(apiOrder => new Order
            {
                    OrderId = Guid.Parse($"{apiOrder.id.ToString().PadLeft(32, '0').Substring(0, 32)}"),
                    CustomerName = apiOrder.title,
                    OrderDate = DateTime.Now,
                    Status = "New"
                }).ToList() ?? new List<Order>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching orders from the external API.");
            return new List<Order>();
        }
    }
}
