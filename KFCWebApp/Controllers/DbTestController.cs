using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KFCWebApp.Models;
using KFCWebApp.Data;

namespace KFCWebApp.Controllers
{
    public class DbTestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbTestController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var result = new List<string>();

            // Kiểm tra tất cả users
            var allUsers = _userManager.Users.ToList();
            result.Add($"Tổng số users: {allUsers.Count}");

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add($"User: {user.Email} - Roles: {string.Join(", ", roles)}");
            }

            // Kiểm tra tài khoản admin cụ thể
            var adminUser = await _userManager.FindByEmailAsync("admin@kfc.com");
            if (adminUser != null)
            {
                var adminRoles = await _userManager.GetRolesAsync(adminUser);
                result.Add($"Admin user tồn tại: {adminUser.Email} - Roles: {string.Join(", ", adminRoles)}");
            }
            else
            {
                result.Add("Admin user không tồn tại");
            }

            return Content(string.Join("\n", result));
        }

        public async Task<IActionResult> FixAdmin()
        {
            var result = new List<string>();

            // Xóa tất cả users có email admin@kfc.com
            var adminUsers = _userManager.Users.Where(u => u.Email == "admin@kfc.com").ToList();
            foreach (var user in adminUsers)
            {
                var deleteResult = await _userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    result.Add($"Đã xóa user: {user.Email}");
                }
                else
                {
                    result.Add($"Lỗi khi xóa user {user.Email}: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                }
            }

            // Tạo lại admin user
            var newAdminUser = new ApplicationUser
            {
                UserName = "admin@kfc.com",
                Email = "admin@kfc.com",
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var createResult = await _userManager.CreateAsync(newAdminUser, "Admin123!");
            if (createResult.Succeeded)
            {
                result.Add("Đã tạo admin user thành công");
                
                // Gán role Admin
                var roleResult = await _userManager.AddToRoleAsync(newAdminUser, "Admin");
                if (roleResult.Succeeded)
                {
                    result.Add("Đã gán role Admin thành công");
                }
                else
                {
                    result.Add($"Lỗi khi gán role Admin: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                result.Add($"Lỗi khi tạo admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            return Content(string.Join("\n", result));
        }

        public async Task<IActionResult> CleanAll()
        {
            var result = new List<string>();

            // Xóa tất cả users
            var allUsers = _userManager.Users.ToList();
            foreach (var user in allUsers)
            {
                var deleteResult = await _userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    result.Add($"Đã xóa user: {user.Email}");
                }
                else
                {
                    result.Add($"Lỗi khi xóa user {user.Email}: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                }
            }

            // Xóa tất cả roles
            var allRoles = _roleManager.Roles.ToList();
            foreach (var role in allRoles)
            {
                var deleteResult = await _roleManager.DeleteAsync(role);
                if (deleteResult.Succeeded)
                {
                    result.Add($"Đã xóa role: {role.Name}");
                }
                else
                {
                    result.Add($"Lỗi khi xóa role {role.Name}: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                }
            }

            // Tạo lại roles
            var adminRole = new IdentityRole("Admin");
            var customerRole = new IdentityRole("Customer");
            
            await _roleManager.CreateAsync(adminRole);
            await _roleManager.CreateAsync(customerRole);
            result.Add("Đã tạo lại roles Admin và Customer");

            // Tạo admin user mới
            var adminUser = new ApplicationUser
            {
                UserName = "admin@kfc.com",
                Email = "admin@kfc.com",
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var createResult = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (createResult.Succeeded)
            {
                result.Add("Đã tạo admin user thành công");
                
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                if (roleResult.Succeeded)
                {
                    result.Add("Đã gán role Admin thành công");
                }
                else
                {
                    result.Add($"Lỗi khi gán role Admin: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                result.Add($"Lỗi khi tạo admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            return Content(string.Join("\n", result));
        }
    }
} 