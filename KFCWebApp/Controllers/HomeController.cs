using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KFCWebApp.Models;
using KFCWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace KFCWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // Lấy 3 sản phẩm nổi bật (ví dụ: theo tên hoặc category)
        var gaRan = _context.Products.FirstOrDefault(p => p.Name.Contains("Gà rán"));
        var burger = _context.Products.FirstOrDefault(p => p.Name.Contains("Burger"));
        var drink = _context.Products.FirstOrDefault(p => p.Name.Contains("Coca") || p.Name.Contains("Pepsi") || p.Name.Contains("Nước"));
        // Lấy sản phẩm Burger cho banner 'Burger mới'
        var burgerSpecial = _context.Products.FirstOrDefault(p => p.Name.Contains("Burger"));
        // Lấy sản phẩm combo cho banner 'Combo gia đình' (giữ nguyên specials cũ để không ảnh hưởng)
        var specials = _context.Products.OrderByDescending(p => p.CreatedAt).Take(2).ToList();
        ViewBag.GaRan = gaRan;
        ViewBag.Burger = burger;
        ViewBag.Drink = drink;
        ViewBag.BurgerSpecial = burgerSpecial;
        ViewBag.Specials = specials;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
