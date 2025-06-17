using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KFCWebApp.Data;
using KFCWebApp.Models;
using System.Text.Json;

namespace KFCWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.CartCount = GetCartCount();
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            if (!product.IsAvailable)
            {
                return Json(new { success = false, message = "Sản phẩm hiện không có sẵn!" });
            }

            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            return Json(new { 
                success = true, 
                message = "Đã thêm vào giỏ hàng",
                cartCount = GetCartCount()
            });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            if (!product.IsAvailable)
            {
                return Json(new { success = false, message = "Sản phẩm hiện không có sẵn!" });
            }

            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                SaveCart(cart);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
                return Json(new { success = true });
            }

            return NotFound();
        }

        // POST: Cart/Clear
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return Json(new { success = true });
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        private int GetCartCount()
        {
            var cart = GetCart();
            return cart.Sum(item => item.Quantity);
        }
    }
} 