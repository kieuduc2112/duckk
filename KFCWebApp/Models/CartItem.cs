namespace KFCWebApp.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int? ComboId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemType { get; set; } = "Product"; // "Product" hoặc "Combo"
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
} 