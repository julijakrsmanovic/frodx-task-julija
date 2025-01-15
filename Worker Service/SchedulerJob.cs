using Microsoft.Extensions.Logging;
using Order_Processing_Worker_Service.EFCore.Models;
using Order_Processing_Worker_Service.Infrastructure;
using Order_Processing_Worker_Service.Infrastructure.Services;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

public class SchedulerJob : IJob
{
    private readonly IApiOrderService _apiOrderService;
    private readonly DbOrderService _dbOrderService;
    private readonly ILogger<SchedulerJob> _logger;

    // Constructor: Inject IApiOrderService, DbOrderService, and ILogger
    public SchedulerJob(IApiOrderService apiOrderService, DbOrderService dbOrderService, ILogger<SchedulerJob> logger)
    {
        _apiOrderService = apiOrderService;
        _dbOrderService = dbOrderService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Scheduler started at {DateTime.Now}");

        try
        {
            // Fetch orders from the API
            var apiOrders = await _apiOrderService.FetchOrdersFromApiAsync();

            if (apiOrders != null && apiOrders.Any())
            {
                // Save orders to the database
                await _dbOrderService.AddOrdersAsync(apiOrders);

                _logger.LogInformation($"Processed {apiOrders.Count} orders.");
            }
            else
            {
                _logger.LogWarning("No orders fetched from the API.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the SchedulerJob.");
        }
        finally
        {
            _logger.LogInformation($"Scheduler ended at {DateTime.Now}");
        }
    }
}
