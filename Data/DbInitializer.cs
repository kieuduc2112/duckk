using KFCWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KFCWebApp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Đảm bảo database được tạo
            await context.Database.EnsureCreatedAsync();

            // Kiểm tra xem đã có dữ liệu chưa
            if (await context.Products.AnyAsync())
            {
                return; // Database đã có dữ liệu
            }

            // Thêm danh mục sản phẩm
            var categoryNames = new[]
            {
                "Gà Rán",
                "Burger",
                "Món Ăn Kèm",
                "Đồ Uống",
                "Tráng Miệng"
            };
            var categoryList = new List<Category>();
            foreach (var name in categoryNames)
            {
                categoryList.Add(new Category { Name = name, IsActive = true, CreatedAt = DateTime.Now });
            }
            await context.Categories.AddRangeAsync(categoryList);
            await context.SaveChangesAsync();

            // Thêm sản phẩm mẫu
            var products = new[]
            {
                new Product
                {
                    Name = "Gà Rán 2 Miếng",
                    Description = "2 miếng gà rán giòn rụm, thơm ngon",
                    Price = 69000,
                    CategoryId = categoryList[0].Id,
                    ImageUrl = "/images/products/ga-ran-2-mieng.jpg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Gà Rán 3 Miếng",
                    Description = "3 miếng gà rán giòn rụm, thơm ngon",
                    Price = 99000,
                    CategoryId = categoryList[0].Id,
                    ImageUrl = "/images/products/ga-ran-3-mieng.jpg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Burger Gà",
                    Description = "Burger gà với sốt đặc biệt",
                    Price = 49000,
                    CategoryId = categoryList[1].Id,
                    ImageUrl = "/images/products/burger-ga.jpg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Khoai Tây Chiên",
                    Description = "Khoai tây chiên giòn rụm",
                    Price = 29000,
                    CategoryId = categoryList[2].Id,
                    ImageUrl = "/images/products/khoai-tay-chien.jpg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Coca Cola",
                    Description = "Nước ngọt Coca Cola",
                    Price = 19000,
                    CategoryId = categoryList[3].Id,
                    ImageUrl = "/images/products/coca-cola.jpg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Kem Vanilla",
                    Description = "Kem vani mềm mịn",
                    Price = 15000,
                    CategoryId = categoryList[4].Id,
                    ImageUrl = "/images/products/kem-vanilla.jpg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // Thêm role mặc định
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Thêm tài khoản admin mặc định
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "admin@kfc.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Address = "123 Admin Street",
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
} 