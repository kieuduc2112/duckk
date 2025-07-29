using KFCWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KFCWebApp.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            // Tạo roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Đã tạo role Admin thành công");
                }
                else
                {
                    logger.LogError("Lỗi khi tạo role Admin: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole("Customer"));
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Đã tạo role Customer thành công");
                }
                else
                {
                    logger.LogError("Lỗi khi tạo role Customer: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            // Tạo admin user
            var adminUser = await userManager.FindByEmailAsync("admin@kfc.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@kfc.com",
                    Email = "admin@kfc.com",
                    EmailConfirmed = true,
                    FullName = "Administrator"
                };
                
                var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createResult.Succeeded)
                {
                    logger.LogInformation("Đã tạo admin user thành công");
                    
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Đã gán role Admin cho user thành công");
                    }
                    else
                    {
                        logger.LogError("Lỗi khi gán role Admin: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogError("Lỗi khi tạo admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("Admin user đã tồn tại");
                
                // Kiểm tra xem user đã có role Admin chưa
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Đã gán role Admin cho user thành công");
                    }
                    else
                    {
                        logger.LogError("Lỗi khi gán role Admin: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Tạo categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Burger", Description = "Các loại burger", IsActive = true },
                    new Category { Name = "Gà rán", Description = "Các món gà rán", IsActive = true },
                    new Category { Name = "Đồ uống", Description = "Các loại đồ uống", IsActive = true },
                    new Category { Name = "Món phụ", Description = "Các món phụ", IsActive = true }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Tạo combo categories
            if (!context.ComboCategories.Any())
            {
                var comboCategories = new List<ComboCategory>
                {
                    new ComboCategory { Name = "Combo Gia Đình", Description = "Combo dành cho gia đình", IsActive = true },
                    new ComboCategory { Name = "Combo Cặp Đôi", Description = "Combo dành cho 2 người", IsActive = true },
                    new ComboCategory { Name = "Combo Cá Nhân", Description = "Combo dành cho 1 người", IsActive = true }
                };
                context.ComboCategories.AddRange(comboCategories);
                await context.SaveChangesAsync();
            }

            // Tạo products
            if (!context.Products.Any())
            {
                var categories = await context.Categories.ToListAsync();
                var products = new List<Product>
                {
                    new Product { Name = "Burger Gà", Description = "Burger gà ngon", Price = 45000, CategoryId = categories[0].Id, ImageUrl = "/images/products/burger-ga.jpg", IsAvailable = true },
                    new Product { Name = "Gà Rán", Description = "Gà rán giòn", Price = 35000, CategoryId = categories[1].Id, ImageUrl = "/images/products/ga-ran.jpg", IsAvailable = true },
                    new Product { Name = "Coca Cola", Description = "Nước ngọt Coca Cola", Price = 15000, CategoryId = categories[2].Id, ImageUrl = "/images/products/coca-cola.jpg", IsAvailable = true },
                    new Product { Name = "Khoai Tây Chiên", Description = "Khoai tây chiên giòn", Price = 25000, CategoryId = categories[3].Id, ImageUrl = "/images/products/khoai-tay.jpg", IsAvailable = true }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            // Tạo combos
            if (!context.Combos.Any())
            {
                var comboCategories = await context.ComboCategories.ToListAsync();
                var products = await context.Products.ToListAsync();
                
                var combos = new List<Combo>
                {
                    new Combo 
                    { 
                        Name = "Combo Gia Đình 4 Người", 
                        Description = "Combo đầy đủ cho gia đình 4 người", 
                        Price = 180000, 
                        OriginalPrice = 200000,
                        ComboCategoryId = comboCategories[0].Id, 
                        ImageUrl = "/images/products/combo-gia-dinh.jpg", 
                        IsAvailable = true 
                    },
                    new Combo 
                    { 
                        Name = "Combo Cặp Đôi", 
                        Description = "Combo lãng mạn cho 2 người", 
                        Price = 120000, 
                        OriginalPrice = 140000,
                        ComboCategoryId = comboCategories[1].Id, 
                        ImageUrl = "/images/products/combo-cap-doi.jpg", 
                        IsAvailable = true 
                    },
                    new Combo 
                    { 
                        Name = "Combo Cá Nhân", 
                        Description = "Combo đơn giản cho 1 người", 
                        Price = 65000, 
                        OriginalPrice = 75000,
                        ComboCategoryId = comboCategories[2].Id, 
                        ImageUrl = "/images/products/combo-ca-nhan.jpg", 
                        IsAvailable = true 
                    }
                };
                context.Combos.AddRange(combos);
                await context.SaveChangesAsync();

                // Thêm sản phẩm vào combo
                var comboItems = new List<ComboItem>
                {
                    // Combo Gia Đình 4 Người
                    new ComboItem { ComboId = combos[0].Id, ProductId = products[0].Id, Quantity = 4 }, // 4 burger gà
                    new ComboItem { ComboId = combos[0].Id, ProductId = products[1].Id, Quantity = 2 }, // 2 gà rán
                    new ComboItem { ComboId = combos[0].Id, ProductId = products[2].Id, Quantity = 4 }, // 4 coca cola
                    new ComboItem { ComboId = combos[0].Id, ProductId = products[3].Id, Quantity = 2 }, // 2 khoai tây chiên
                    
                    // Combo Cặp Đôi
                    new ComboItem { ComboId = combos[1].Id, ProductId = products[0].Id, Quantity = 2 }, // 2 burger gà
                    new ComboItem { ComboId = combos[1].Id, ProductId = products[1].Id, Quantity = 1 }, // 1 gà rán
                    new ComboItem { ComboId = combos[1].Id, ProductId = products[2].Id, Quantity = 2 }, // 2 coca cola
                    new ComboItem { ComboId = combos[1].Id, ProductId = products[3].Id, Quantity = 1 }, // 1 khoai tây chiên
                    
                    // Combo Cá Nhân
                    new ComboItem { ComboId = combos[2].Id, ProductId = products[0].Id, Quantity = 1 }, // 1 burger gà
                    new ComboItem { ComboId = combos[2].Id, ProductId = products[2].Id, Quantity = 1 }, // 1 coca cola
                    new ComboItem { ComboId = combos[2].Id, ProductId = products[3].Id, Quantity = 1 }  // 1 khoai tây chiên
                };
                context.ComboItems.AddRange(comboItems);
                await context.SaveChangesAsync();
            }

            // Tạo data giả cho đơn hàng và doanh thu
            if (!context.Orders.Any() || context.Orders.Count() < 10) // Tạo nếu chưa có hoặc ít hơn 10 đơn hàng
            {
                try
                {
                    var adminUserForOrders = await userManager.FindByEmailAsync("admin@kfc.com");
                    if (adminUserForOrders == null)
                    {
                        logger.LogWarning("Không tìm thấy admin user để tạo data giả");
                        return;
                    }

                    var products = await context.Products.ToListAsync();
                    var combos = await context.Combos.ToListAsync();
                    
                    if (!products.Any() && !combos.Any())
                    {
                        logger.LogWarning("Không có sản phẩm hoặc combo để tạo data giả");
                        return;
                    }
                    
                    // Tạo đơn hàng trong 30 ngày gần đây
                    var random = new Random();
                    var orders = new List<Order>();
                    
                    for (int i = 0; i < 20; i++) // Giảm xuống 20 đơn hàng để tránh lỗi
                    {
                        var orderDate = DateTime.Now.AddDays(-random.Next(0, 30)); // Ngẫu nhiên trong 30 ngày qua
                        var totalAmount = random.Next(50000, 500000); // Doanh thu từ 50k đến 500k
                        
                        var order = new Order
                        {
                            ApplicationUserId = adminUserForOrders.Id,
                            CustomerName = $"Khách hàng {i + 1}",
                            Email = $"customer{i + 1}@example.com",
                            PhoneNumber = $"0{random.Next(900000000, 999999999)}",
                            Address = $"Địa chỉ {i + 1}, TP.HCM",
                            TotalAmount = totalAmount,
                            Status = OrderStatus.Completed, // Đơn hàng đã hoàn thành
                            CreatedAt = orderDate,
                            UpdatedAt = orderDate
                        };
                        orders.Add(order);
                    }
                    
                    context.Orders.AddRange(orders);
                    await context.SaveChangesAsync();

                    // Tạo chi tiết đơn hàng
                    var orderDetails = new List<OrderDetail>();
                    
                    foreach (var order in orders)
                    {
                        var numItems = random.Next(1, 4); // 1-3 món trong mỗi đơn
                        
                        for (int j = 0; j < numItems; j++)
                        {
                            var isProduct = random.Next(2) == 0; // 50% là sản phẩm, 50% là combo
                            
                            if (isProduct && products.Any())
                            {
                                var product = products[random.Next(products.Count)];
                                var quantity = random.Next(1, 3);
                                
                                orderDetails.Add(new OrderDetail
                                {
                                    OrderId = order.Id,
                                    ProductId = product.Id,
                                    ItemName = product.Name,
                                    ItemType = "Product",
                                    Price = product.Price,
                                    Quantity = quantity
                                });
                            }
                            else if (combos.Any())
                            {
                                var combo = combos[random.Next(combos.Count)];
                                var quantity = random.Next(1, 2);
                                
                                orderDetails.Add(new OrderDetail
                                {
                                    OrderId = order.Id,
                                    ComboId = combo.Id,
                                    ItemName = combo.Name,
                                    ItemType = "Combo",
                                    Price = combo.Price,
                                    Quantity = quantity
                                });
                            }
                        }
                    }
                    
                    context.OrderDetails.AddRange(orderDetails);
                    await context.SaveChangesAsync();
                    
                    logger.LogInformation("Đã tạo thành công {OrderCount} đơn hàng và {DetailCount} chi tiết đơn hàng", orders.Count, orderDetails.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi tạo data giả cho đơn hàng");
                }
            }
        }
    }
} 