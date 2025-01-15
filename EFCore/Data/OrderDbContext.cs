using Microsoft.EntityFrameworkCore;
using Order_Processing_Worker_Service.EFCore.Models;
using System.Collections.Generic;

namespace Order_Processing_Worker_Service.EFCore.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
