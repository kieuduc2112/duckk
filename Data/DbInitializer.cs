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
                    Description = "2 miếng gà rán giòn rụm, thơm ngon với lớp vỏ giòn và thịt mềm bên trong",
                    Price = 69000,
                    CategoryId = categoryList[0].Id,
                    ImageUrl = "/images/products/ga-ran.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Gà Rán 3 Miếng",
                    Description = "3 miếng gà rán giòn rụm, thơm ngon với sốt đặc biệt",
                    Price = 99000,
                    CategoryId = categoryList[0].Id,
                    ImageUrl = "/images/products/ga-ran.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Gà Nướng BBQ",
                    Description = "Gà nướng với sốt BBQ đặc biệt, thịt mềm và thơm",
                    Price = 89000,
                    CategoryId = categoryList[0].Id,
                    ImageUrl = "/images/products/ga-nuong.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Burger Gà",
                    Description = "Burger gà với sốt đặc biệt, rau tươi và phô mai",
                    Price = 49000,
                    CategoryId = categoryList[1].Id,
                    ImageUrl = "/images/products/burger.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Burger Bò",
                    Description = "Burger bò với thịt bò tươi ngon, rau và sốt đặc biệt",
                    Price = 59000,
                    CategoryId = categoryList[1].Id,
                    ImageUrl = "/images/products/burger.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Combo Gà Rán",
                    Description = "Combo gà rán với khoai tây chiên và nước ngọt",
                    Price = 129000,
                    CategoryId = categoryList[1].Id,
                    ImageUrl = "/images/products/combo.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Khoai Tây Chiên",
                    Description = "Khoai tây chiên giòn rụm với muối và gia vị đặc biệt",
                    Price = 29000,
                    CategoryId = categoryList[2].Id,
                    ImageUrl = "/images/products/khoai-tay.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Khoai Tây Nghiền",
                    Description = "Khoai tây nghiền mềm mịn với bơ và sữa",
                    Price = 25000,
                    CategoryId = categoryList[2].Id,
                    ImageUrl = "/images/products/khoai-tay.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Coca Cola",
                    Description = "Nước ngọt Coca Cola mát lạnh",
                    Price = 19000,
                    CategoryId = categoryList[3].Id,
                    ImageUrl = "/images/products/nuoc-ngot.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Pepsi",
                    Description = "Nước ngọt Pepsi mát lạnh",
                    Price = 19000,
                    CategoryId = categoryList[3].Id,
                    ImageUrl = "/images/products/nuoc-ngot.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Kem Vanilla",
                    Description = "Kem vani mềm mịn với hương vị tự nhiên",
                    Price = 15000,
                    CategoryId = categoryList[4].Id,
                    ImageUrl = "/images/products/kem.svg",
                    IsAvailable = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Kem Chocolate",
                    Description = "Kem chocolate đậm đà với hương vị cacao",
                    Price = 17000,
                    CategoryId = categoryList[4].Id,
                    ImageUrl = "/images/products/kem.svg",
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