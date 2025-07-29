using Microsoft.AspNetCore.Identity;
using System;

namespace KFCWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
} 