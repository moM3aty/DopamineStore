using DopamineStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DopamineStore.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<ProductImg> ProductImgs { get; set; }

        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<ShippingZone> ShippingZones { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

    }
}
