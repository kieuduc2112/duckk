using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KFCWebApp.Models;
using System.Threading.Tasks;

namespace KFCWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Gán role Khách hàng mặc định
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
} 