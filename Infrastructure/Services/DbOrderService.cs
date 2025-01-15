using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order_Processing_Worker_Service.EFCore.Data;
using Order_Processing_Worker_Service.EFCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order_Processing_Worker_Service.Infrastructure.Services
{
    public class DbOrderService : IDbOrderService
    {
        private readonly OrderDbContext _dbContext;
        private readonly ILogger<DbOrderService> _logger;

        public DbOrderService(OrderDbContext dbContext, ILogger<DbOrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task AddOrdersAsync(List<Order> orders)
        {
            try
            {
                _logger.LogInformation("Adding orders to the database.");
                await _dbContext.Orders.AddRangeAsync(orders);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Orders added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding orders to the database.");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner Exception details:");
                }
            }
        }
    }
}
