using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Models;

namespace KFCWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboCategory> ComboCategories { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponCategory> CouponCategories { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ProductPromotion> ProductPromotions { get; set; }
        public DbSet<ComboPromotion> ComboPromotions { get; set; }
        public DbSet<ComboPromotionItem> ComboPromotionItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .Property(p => p.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình OrderDetail có thể tham chiếu đến Product hoặc Combo
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderDetail>()
                .HasOne(od => od.Combo)
                .WithMany()
                .HasForeignKey(od => od.ComboId)
                .OnDelete(DeleteBehavior.Restrict);

            // Đảm bảo rằng chỉ một trong hai foreign key được sử dụng
            builder.Entity<OrderDetail>()
                .ToTable(t => t.HasCheckConstraint("CK_OrderDetail_ProductOrCombo", 
                    "(ProductId IS NOT NULL AND ComboId IS NULL) OR (ProductId IS NULL AND ComboId IS NOT NULL)"));

            builder.Entity<Combo>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Entity<Combo>()
                .Property(c => c.OriginalPrice)
                .HasPrecision(18, 2);

            builder.Entity<Combo>()
                .HasOne(c => c.ComboCategory)
                .WithMany(cc => cc.Combos)
                .HasForeignKey(c => c.ComboCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Combo)
                .WithMany(c => c.ComboItems)
                .HasForeignKey(ci => ci.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CouponCategory>().ToTable("CouponCategories");

            // Cấu hình precision cho decimal properties
            builder.Entity<ProductPromotion>()
                .Property(pp => pp.PercentDiscount)
                .HasPrecision(18, 2);

            builder.Entity<ProductPromotion>()
                .Property(pp => pp.AmountDiscount)
                .HasPrecision(18, 2);

            builder.Entity<ComboPromotion>()
                .Property(cp => cp.RewardValue)
                .HasPrecision(18, 2);

            builder.Entity<Coupon>()
                .Property(c => c.Value)
                .HasPrecision(18, 2);
        }
    }
} 