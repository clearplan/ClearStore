using ClearStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductApparel> ProductApparel { get; set; } = null!;
        public virtual DbSet<ProductCart> ProductCarts { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProductColor> ProductColors { get; set; } = null!;
        public virtual DbSet<ProductColorCategory> ProductColorCategories { get; set; } = null!;
        public virtual DbSet<ProductGender> ProductGenders { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductInventory> ProductInventory { get; set; } = null!;
        public virtual DbSet<ProductItem> ProductItems { get; set; } = null!;
        public virtual DbSet<ProductOffice> ProductOffices { get; set; } = null!;
        public virtual DbSet<ProductOrder> ProductOrders { get; set; } = null!;
        public virtual DbSet<ProductOrderStatusCategory> ProductOrderStatusCategories { get; set; } = null!;
        public virtual DbSet<ProductRating> ProductRatings { get; set; } = null!;
        public virtual DbSet<ProductSize> ProductSizes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=StoreContext");
            }
        }
    }
}
