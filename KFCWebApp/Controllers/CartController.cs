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

            // Gợi ý khuyến mãi sản phẩm lẻ (ProductPromotion)
            var now = DateTime.Now;
            var productPromos = _context.ProductPromotions
                .Include(p => p.Product1)
                .Include(p => p.Product2)
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .ToList();
            var suggestPromos = new List<dynamic>();
            foreach (var promo in productPromos)
            {
                var has1 = cart.Any(i => i.ItemType == "Product" && i.ProductId == promo.ProductId1);
                var has2 = cart.Any(i => i.ItemType == "Product" && i.ProductId == promo.ProductId2);
                if (has1 && !has2 && promo.Product2 != null)
                {
                    suggestPromos.Add(new {
                        Product = promo.Product2,
                        Promo = promo,
                        MissingProduct = promo.Product2,
                        OwnedProduct = promo.Product1,
                        PromoType = "ProductPromotion"
                    });
                }
                else if (!has1 && has2 && promo.Product1 != null)
                {
                    suggestPromos.Add(new {
                        Product = promo.Product1,
                        Promo = promo,
                        MissingProduct = promo.Product1,
                        OwnedProduct = promo.Product2,
                        PromoType = "ProductPromotion"
                    });
                }
            }

            // Gợi ý khuyến mãi động (Promotion dạng JSON, TwoProductPromotion)
            var dynamicPromos = _context.Promotions
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now && p.ConditionType == "TwoProductPromotion")
                .ToList();
            foreach (var promo in dynamicPromos)
            {
                try
                {
                    var cond = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(promo.ConditionJson);
                    if (cond != null && cond.ContainsKey("ProductId1") && cond.ContainsKey("ProductId2"))
                    {
                        int.TryParse(cond["ProductId1"], out int pid1);
                        int.TryParse(cond["ProductId2"], out int pid2);
                        var p1 = _context.Products.FirstOrDefault(p => p.Id == pid1);
                        var p2 = _context.Products.FirstOrDefault(p => p.Id == pid2);
                        var has1 = cart.Any(i => i.ItemType == "Product" && i.ProductId == pid1);
                        var has2 = cart.Any(i => i.ItemType == "Product" && i.ProductId == pid2);
                        if (p1 != null && p2 != null)
                        {
                            if (has1 && !has2)
                            {
                                suggestPromos.Add(new {
                                    Product = p2,
                                    Promo = promo,
                                    MissingProduct = p2,
                                    OwnedProduct = p1,
                                    PromoType = "PromotionJSON"
                                });
                            }
                            else if (!has1 && has2)
                            {
                                suggestPromos.Add(new {
                                    Product = p1,
                                    Promo = promo,
                                    MissingProduct = p1,
                                    OwnedProduct = p2,
                                    PromoType = "PromotionJSON"
                                });
                            }
                        }
                    }
                }
                catch { }
            }

            ViewBag.SuggestPromos = suggestPromos;
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int? productId = null, int? comboId = null, int quantity = 1)
        {
            if (productId.HasValue)
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
                var cartItem = cart.FirstOrDefault(i => i.ProductId == productId && i.ItemType == "Product");

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ItemName = product.Name,
                        ItemType = "Product",
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = quantity
                    });
                }

                ApplyDynamicPromotion(cart);
                SaveCart(cart);
                return Json(new { 
                    success = true, 
                    message = "Đã thêm vào giỏ hàng",
                    cartCount = GetCartCount()
                });
            }
            else if (comboId.HasValue)
            {
                var combo = await _context.Combos.FindAsync(comboId);
                if (combo == null)
                {
                    return Json(new { success = false, message = "Combo không tồn tại!" });
                }

                if (!combo.IsAvailable)
                {
                    return Json(new { success = false, message = "Combo hiện không có sẵn!" });
                }

                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
                }

                var cart = GetCart();
                var cartItem = cart.FirstOrDefault(i => i.ComboId == comboId && i.ItemType == "Combo");

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ComboId = combo.Id,
                        ItemName = combo.Name,
                        ItemType = "Combo",
                        ImageUrl = combo.ImageUrl,
                        Price = combo.Price,
                        Quantity = quantity
                    });
                }

                ApplyDynamicPromotion(cart);
                SaveCart(cart);
                return Json(new { 
                    success = true, 
                    message = "Đã thêm combo vào giỏ hàng",
                    cartCount = GetCartCount()
                });
            }

            return Json(new { success = false, message = "Tham số không hợp lệ!" });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int? productId = null, int? comboId = null, int quantity = 1)
        {
            if (productId.HasValue)
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
                var cartItem = cart.FirstOrDefault(i => i.ProductId == productId && i.ItemType == "Product");

                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    ApplyDynamicPromotion(cart);
                    SaveCart(cart);
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
            }
            else if (comboId.HasValue)
            {
                var combo = await _context.Combos.FindAsync(comboId);
                if (combo == null)
                {
                    return Json(new { success = false, message = "Combo không tồn tại!" });
                }

                if (!combo.IsAvailable)
                {
                    return Json(new { success = false, message = "Combo hiện không có sẵn!" });
                }

                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
                }

                var cart = GetCart();
                var cartItem = cart.FirstOrDefault(i => i.ComboId == comboId && i.ItemType == "Combo");

                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    ApplyDynamicPromotion(cart);
                    SaveCart(cart);
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Không tìm thấy combo trong giỏ hàng!" });
            }

            return Json(new { success = false, message = "Tham số không hợp lệ!" });
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public IActionResult RemoveItem(int? productId = null, int? comboId = null)
        {
            var cart = GetCart();
            CartItem? cartItem = null;

            if (productId.HasValue)
            {
                cartItem = cart.FirstOrDefault(i => i.ProductId == productId && i.ItemType == "Product");
            }
            else if (comboId.HasValue)
            {
                cartItem = cart.FirstOrDefault(i => i.ComboId == comboId && i.ItemType == "Combo");
            }

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                ApplyDynamicPromotion(cart);
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
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            
            try
            {
                var result = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                return result ?? new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
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

        private void ApplyDynamicPromotion(List<CartItem> cart)
        {
            var now = DateTime.Now;
            // Xóa các dòng giảm giá cũ
            cart.RemoveAll(i => i.ItemType == "PercentDiscount" || i.ItemType == "Discount" || (i.ItemType == "Product" && i.ItemName.Contains("(Tặng)")));

            // Áp dụng khuyến mãi sản phẩm lẻ (ProductPromotion)
            var productPromos = _context.ProductPromotions.Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now).ToList();
            foreach (var promo in productPromos)
            {
                var item1 = cart.FirstOrDefault(i => i.ItemType == "Product" && i.ProductId == promo.ProductId1);
                var item2 = cart.FirstOrDefault(i => i.ItemType == "Product" && i.ProductId == promo.ProductId2);
                if (item1 != null && item2 != null)
                {
                    // Áp dụng giảm giá cho cả 2 sản phẩm nếu đủ cả 2 trong giỏ
                    if (promo.PercentDiscount.HasValue && promo.PercentDiscount.Value > 0)
                    {
                        decimal discount1 = Math.Round(item1.Price * promo.PercentDiscount.Value / 100m, 0);
                        decimal discount2 = Math.Round(item2.Price * promo.PercentDiscount.Value / 100m, 0);
                        if (discount1 > 0)
                        {
                            cart.Add(new CartItem
                            {
                                ItemName = $"Giảm {promo.PercentDiscount.Value}% cho {item1.ItemName}",
                                ItemType = "PercentDiscount",
                                ProductId = item1.ProductId,
                                Price = -discount1,
                                Quantity = item1.Quantity
                            });
                        }
                        if (discount2 > 0)
                        {
                            cart.Add(new CartItem
                            {
                                ItemName = $"Giảm {promo.PercentDiscount.Value}% cho {item2.ItemName}",
                                ItemType = "PercentDiscount",
                                ProductId = item2.ProductId,
                                Price = -discount2,
                                Quantity = item2.Quantity
                            });
                        }
                    }
                    else if (promo.AmountDiscount.HasValue && promo.AmountDiscount.Value > 0)
                    {
                        decimal discount = promo.AmountDiscount.Value;
                        if (discount > 0)
                        {
                            // Áp dụng giảm tiền cho cả 2 sản phẩm
                            cart.Add(new CartItem
                            {
                                ItemName = $"Giảm {discount:N0}đ cho {item1.ItemName} & {item2.ItemName}",
                                ItemType = "Discount",
                                ProductId = item1.ProductId, // hoặc null
                                Price = -discount,
                                Quantity = 1
                            });
                        }
                    }
                }
            }

            // Áp dụng khuyến mãi combo (ComboPromotion)
            var comboPromos = _context.ComboPromotions.Include(c => c.Items).Where(c => c.IsActive && c.StartDate <= now && c.EndDate >= now).ToList();
            foreach (var promo in comboPromos)
            {
                // Kiểm tra đủ điều kiện combo (tất cả sản phẩm và số lượng)
                bool enough = promo.Items.All(cp =>
                    cart.Any(i => i.ItemType == "Product" && i.ProductId == cp.ProductId && i.Quantity >= cp.Quantity)
                );
                if (enough)
                {
                    // Tính tổng giá trị các sản phẩm combo
                    decimal total = 0;
                    foreach (var cp in promo.Items)
                    {
                        var item = cart.FirstOrDefault(i => i.ItemType == "Product" && i.ProductId == cp.ProductId);
                        if (item != null) total += item.Price * cp.Quantity;
                    }
                    if (promo.RewardType == "Percent" && promo.RewardValue.HasValue)
                    {
                        var discount = Math.Round(total * promo.RewardValue.Value / 100, 0);
                        if (discount > 0)
                        {
                            cart.Add(new CartItem
                            {
                                ItemType = "Discount",
                                ItemName = $"{promo.Name} - Giảm {promo.RewardValue.Value}%",
                                Price = -discount,
                                Quantity = 1
                            });
                        }
                    }
                    else if (promo.RewardType == "Amount" && promo.RewardValue.HasValue)
                    {
                        var discount = promo.RewardValue.Value;
                        if (discount > 0)
                        {
                            cart.Add(new CartItem
                            {
                                ItemType = "Discount",
                                ItemName = $"{promo.Name} - Giảm {discount:N0}đ",
                                Price = -discount,
                                Quantity = 1
                            });
                        }
                    }
                    else if (promo.RewardType == "Gift" && promo.RewardProductId.HasValue)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == promo.RewardProductId.Value);
                        if (product != null)
                        {
                            cart.Add(new CartItem
                            {
                                ProductId = product.Id,
                                ItemName = product.Name + " (Tặng)",
                                ItemType = "Product",
                                ImageUrl = product.ImageUrl,
                                Price = 0,
                                Quantity = 1
                            });
                        }
                    }
                }
            }

            // Áp dụng các khuyến mãi động cũ (Promotions bảng JSON)
            var promotions = _context.Promotions.Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now).ToList();
            foreach (var promo in promotions)
            {
                if (promo.ConditionType == "BuyXGetY")
                {
                    try
                    {
                        var cond = JsonSerializer.Deserialize<BuyXGetYCondition>(promo.ConditionJson);
                        var reward = JsonSerializer.Deserialize<GiftReward>(promo.RewardJson);
                        if (cond == null || reward == null) continue;
                        int times = int.MaxValue;
                        foreach (var buy in cond.buy)
                        {
                            // Tổng số lượng tất cả sản phẩm trong giỏ thuộc category này
                            int totalQty = cart.Where(i => i.ItemType == "Product")
                                .Join(_context.Products, ci => ci.ProductId, p => p.Id, (ci, p) => new { ci, p })
                                .Where(x => x.p.CategoryId == buy.productCategoryId)
                                .Sum(x => x.ci.Quantity);
                            if (totalQty < buy.quantity)
                            {
                                times = 0;
                                break;
                            }
                            times = Math.Min(times, totalQty / buy.quantity);
                        }
                        var gift = cart.FirstOrDefault(i => i.ProductId == reward.productId && i.Price == 0 && i.ItemType == "Product");
                        if (times > 0)
                        {
                            if (gift == null)
                            {
                                var product = _context.Products.FirstOrDefault(p => p.Id == reward.productId);
                                if (product != null)
                                {
                                    cart.Add(new CartItem
                                    {
                                        ProductId = product.Id,
                                        ItemName = product.Name + " (Tặng)",
                                        ItemType = "Product",
                                        ImageUrl = product.ImageUrl,
                                        Price = 0,
                                        Quantity = reward.quantity * times
                                    });
                                }
                            }
                            else
                            {
                                gift.Quantity = reward.quantity * times;
                            }
                        }
                        else
                        {
                            if (gift != null)
                            {
                                cart.Remove(gift);
                            }
                        }
                    }
                    catch { }
                }
                else if (promo.ConditionType == "AmountDiscount")
                {
                    try
                    {
                        var cond = JsonSerializer.Deserialize<AmountDiscountCondition>(promo.ConditionJson);
                        var reward = JsonSerializer.Deserialize<AmountDiscountReward>(promo.RewardJson);
                        if (cond == null || reward == null) continue;
                        // Kiểm tra tổng tiền giỏ hàng có đạt minAmount không
                        decimal subtotal = cart.Where(i => i.Price > 0).Sum(i => i.Total);
                        var discountItem = cart.FirstOrDefault(i => i.ItemType == "Discount" && i.ItemName == promo.Name);
                        if (subtotal >= cond.minAmount)
                        {
                            if (discountItem == null)
                            {
                                cart.Add(new CartItem
                                {
                                    ItemType = "Discount",
                                    ItemName = promo.Name,
                                    Price = -reward.amount,
                                    Quantity = 1
                                });
                            }
                            else
                            {
                                discountItem.Price = -reward.amount;
                            }
                        }
                        else
                        {
                            if (discountItem != null)
                            {
                                cart.Remove(discountItem);
                            }
                        }
                    }
                    catch { }
                }
                else if (promo.ConditionType == "BuyProductsPercentDiscount")
                {
                    try
                    {
                        var cond = System.Text.Json.JsonSerializer.Deserialize<BuyProductsPercentDiscountCondition>(promo.ConditionJson);
                        var reward = System.Text.Json.JsonSerializer.Deserialize<PercentDiscountReward>(promo.RewardJson);
                        if (cond == null || reward == null) continue;
                        bool enough = true;
                        decimal total = 0;
                        foreach (var p in cond.products)
                        {
                            var item = cart.FirstOrDefault(i => i.ProductId == p.productId && i.ItemType == "Product");
                            if (item == null || item.Quantity < p.quantity) { enough = false; break; }
                            total += item.Price * p.quantity;
                        }
                        var discountItem = cart.FirstOrDefault(i => i.ItemType == "PercentDiscount" && i.ProductId == null && i.ComboId == null && i.Price < 0);
                        if (enough)
                        {
                            decimal discount = Math.Round(total * reward.percent / 100m, 0);
                            if (discount > 0)
                            {
                                if (discountItem == null)
                                {
                                    cart.Add(new CartItem
                                    {
                                        ItemName = $"Khuyến mãi giảm {reward.percent}%",
                                        ItemType = "PercentDiscount",
                                        Price = -discount,
                                        Quantity = 1
                                    });
                                }
                                else
                                {
                                    discountItem.Price = -discount;
                                    discountItem.ItemName = $"Khuyến mãi giảm {reward.percent}%";
                                }
                            }
                        }
                        else if (discountItem != null)
                        {
                            cart.Remove(discountItem);
                        }
                    }
                    catch { }
                }
                else if (promo.ConditionType == "ProductPercentDiscount")
                {
                    try
                    {
                        var cond = System.Text.Json.JsonSerializer.Deserialize<ProductPercentDiscountCondition>(promo.ConditionJson);
                        var reward = System.Text.Json.JsonSerializer.Deserialize<PercentDiscountReward>(promo.RewardJson);
                        if (cond == null || reward == null) continue;
                        foreach (var item in cart.Where(i => i.ItemType == "Product" && cond.products.Contains(i.ProductId)))
                        {
                            // Kiểm tra đã có discount chưa
                            var discountItem = cart.FirstOrDefault(d => d.ItemType == "PercentDiscount" && d.ProductId == item.ProductId);
                            decimal discount = Math.Round(item.Price * reward.percent / 100m, 0);
                            if (discount > 0)
                            {
                                if (discountItem == null)
                                {
                                    cart.Add(new CartItem
                                    {
                                        ItemName = $"Giảm {reward.percent}% cho {item.ItemName}",
                                        ItemType = "PercentDiscount",
                                        ProductId = item.ProductId,
                                        Price = -discount,
                                        Quantity = item.Quantity
                                    });
                                }
                                else
                                {
                                    discountItem.Price = -discount;
                                    discountItem.Quantity = item.Quantity;
                                    discountItem.ItemName = $"Giảm {reward.percent}% cho {item.ItemName}";
                                }
                            }
                        }
                        // Xóa discount nếu sản phẩm không còn trong giỏ
                        var toRemove = cart.Where(i => i.ItemType == "PercentDiscount" && i.ProductId != null && !cart.Any(p => p.ProductId == i.ProductId && p.ItemType == "Product")).ToList();
                        foreach (var d in toRemove) cart.Remove(d);
                    }
                    catch { }
                }
                else if (promo.ConditionType == "BuyProductsComboPercentDiscount")
                {
                    try
                    {
                        var cond = System.Text.Json.JsonSerializer.Deserialize<BuyProductsPercentDiscountCondition>(promo.ConditionJson);
                        var reward = System.Text.Json.JsonSerializer.Deserialize<PercentDiscountReward>(promo.RewardJson);
                        if (cond == null || reward == null) continue;
                        // Kiểm tra đủ từng sản phẩm và số lượng
                        bool enough = cond.products.All(cp =>
                            cart.Any(i => i.ItemType == "Product" && i.ProductId == cp.productId && i.Quantity >= cp.quantity)
                        );
                        if (enough)
                        {
                            // Tính tổng giá trị các sản phẩm combo
                            decimal total = 0;
                            foreach (var cp in cond.products)
                            {
                                var item = cart.FirstOrDefault(i => i.ItemType == "Product" && i.ProductId == cp.productId);
                                if (item != null) total += item.Price * cp.quantity;
                            }
                            var discount = Math.Round(total * reward.percent / 100, 0);
                            // Thêm dòng giảm giá combo nếu chưa có
                            if (!cart.Any(i => i.ItemType == "Discount" && i.ItemName == $"Khuyến mãi combo giảm {reward.percent}%"))
                            {
                                cart.Add(new CartItem
                                {
                                    ItemType = "Discount",
                                    ItemName = $"Khuyến mãi combo giảm {reward.percent}%",
                                    Price = -discount,
                                    Quantity = 1
                                });
                            }
                        }
                        else
                        {
                            // Nếu không đủ điều kiện, xóa discount combo nếu có
                            cart.RemoveAll(i => i.ItemType == "Discount" && i.ItemName.StartsWith("Khuyến mãi combo giảm"));
                        }
                    }
                    catch { }
                }
                // Áp dụng khuyến mãi 2 sản phẩm cụ thể (TwoProductPromotion)
                else if (promo.ConditionType == "TwoProductPromotion")
                {
                    try
                    {
                        // Thử parse kiểu mới
                        TwoProductCondition? cond = null;
                        TwoProductReward? reward = null;
                        try {
                            cond = System.Text.Json.JsonSerializer.Deserialize<TwoProductCondition>(promo.ConditionJson);
                            reward = System.Text.Json.JsonSerializer.Deserialize<TwoProductReward>(promo.RewardJson);
                        } catch {}
                        // Nếu không parse được thì thử kiểu cũ (ProductId1, ProductId2 là string)
                        if (cond == null || cond.Product1Id == 0 || cond.Product2Id == 0)
                        {
                            var legacyCond = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(promo.ConditionJson);
                            if (legacyCond != null && legacyCond.ContainsKey("ProductId1") && legacyCond.ContainsKey("ProductId2"))
                            {
                                cond = new TwoProductCondition {
                                    Product1Id = int.TryParse(legacyCond["ProductId1"], out var p1) ? p1 : 0,
                                    Product2Id = int.TryParse(legacyCond["ProductId2"], out var p2) ? p2 : 0
                                };
                            }
                        }
                        if (reward == null || reward.Percent == 0)
                        {
                            var legacyReward = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(promo.RewardJson);
                            if (legacyReward != null && legacyReward.ContainsKey("PercentDiscount"))
                            {
                                reward = new TwoProductReward {
                                    Percent = int.TryParse(legacyReward["PercentDiscount"], out var percent) ? percent : 0
                                };
                            }
                        }
                        if (cond == null || reward == null) continue;
                        var item1 = cart.FirstOrDefault(i => i.ItemType == "Product" && i.ProductId == cond.Product1Id);
                        var item2 = cart.FirstOrDefault(i => i.ItemType == "Product" && i.ProductId == cond.Product2Id);
                        if (item1 != null && item2 != null)
                        {
                            // Kiểm tra đã có discount chưa
                            var discountItem = cart.FirstOrDefault(d => d.ItemType == "PercentDiscount" && d.ProductId == null && d.ComboId == null && d.ItemName == $"Khuyến mãi 2 sản phẩm giảm {reward.Percent}%");
                            decimal total = (item1.Price * item1.Quantity) + (item2.Price * item2.Quantity);
                            decimal discount = Math.Round(total * reward.Percent / 100m, 0);
                            if (discount > 0)
                            {
                                if (discountItem == null)
                                {
                                    cart.Add(new CartItem
                                    {
                                        ItemName = $"Khuyến mãi 2 sản phẩm giảm {reward.Percent}%",
                                        ItemType = "PercentDiscount",
                                        Price = -discount,
                                        Quantity = 1
                                    });
                                }
                                else
                                {
                                    discountItem.Price = -discount;
                                }
                            }
                        }
                        else
                        {
                            // Nếu không đủ điều kiện, xóa discount nếu có
                            cart.RemoveAll(i => i.ItemType == "PercentDiscount" && i.ItemName == $"Khuyến mãi 2 sản phẩm giảm {reward.Percent}%");
                        }
                    }
                    catch { }
                }
                // Có thể mở rộng thêm các loại khuyến mãi khác ở đây
            }
        }

        // Các class phụ trợ cho deserialize JSON
        private class BuyXGetYCondition
        {
            public string type { get; set; } = string.Empty;
            public List<BuyItem> buy { get; set; } = new();
        }
        private class BuyItem
        {
            public int productCategoryId { get; set; }
            public int quantity { get; set; }
        }
        private class GiftReward
        {
            public string type { get; set; } = string.Empty;
            public int productId { get; set; }
            public int quantity { get; set; }
        }
        private class AmountDiscountCondition
        {
            public string type { get; set; } = string.Empty;
            public decimal minAmount { get; set; } = 0;
        }
        private class AmountDiscountReward
        {
            public string type { get; set; } = string.Empty;
            public decimal amount { get; set; } = 0;
        }
        public class BuyProductsPercentDiscountCondition
        {
            public string type { get; set; } = string.Empty;
            public List<ProductQty> products { get; set; } = new();
            public class ProductQty { public int productId { get; set; } public int quantity { get; set; } }
        }
        public class PercentDiscountReward
        {
            public string type { get; set; } = string.Empty;
            public int percent { get; set; }
        }
        public class ProductPercentDiscountCondition
        {
            public string type { get; set; } = string.Empty;
            public List<int> products { get; set; } = new();
        }
        // Thêm class phụ trợ cho khuyến mãi 2 sản phẩm
        private class TwoProductCondition
        {
            public int Product1Id { get; set; }
            public int Product2Id { get; set; }
        }
        private class TwoProductReward
        {
            public int Percent { get; set; }
        }
    }
} 