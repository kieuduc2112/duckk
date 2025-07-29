using System.ComponentModel.DataAnnotations;

namespace KFCWebApp.Models
{
    public class ComboPromotionItem
    {
        public int Id { get; set; }
        public int ComboPromotionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
} 