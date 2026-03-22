using Microsoft.EntityFrameworkCore;
using WorkOrders.Api.Models;

namespace WorkOrders.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<Cost> Costs { get; set; }
}