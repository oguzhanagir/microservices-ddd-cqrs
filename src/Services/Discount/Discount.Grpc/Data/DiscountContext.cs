using Discount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public class DiscountContext : DbContext
{
    public DbSet<Coupon> Coupons { get; set; } = default!;

    public DiscountContext(DbContextOptions options) : base(options)
    {
    }

    protected DiscountContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coupon>().HasData(
            new Coupon
            {
                Id = 1,
                ProductName = "IPhone X",
                Description = "IPhone Discount",
                Amount = 150
            },
            new Coupon
            {
                Id = 2,
                ProductName = "Samsung 10",
                Description = "Samsung Discount",
                Amount = 100
            },
            new Coupon
            {
                Id = 3,
                ProductName = "Google Pixel",
                Description = "Google Discount",
                Amount = 50
            }
        );
    }
}
