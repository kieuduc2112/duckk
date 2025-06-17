using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFCWebApp.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
} 