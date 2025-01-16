using ECommerceShopingCartASPNET8.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShopingCartASPNET8.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }    // Already existing
        public DbSet<Cart> Carts { get; set; }          // New DbSet for Carts
        public DbSet<Item> CartItems { get; set; }      // New DbSet for Items (CartItems)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships if necessary
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CartId);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);
            modelBuilder.Entity<Cart>()
            .Property(c => c.Id)
            .HasDefaultValueSql("NEWID()");

        }
    }

}
