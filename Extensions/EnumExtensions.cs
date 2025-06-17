using System.ComponentModel.DataAnnotations;
using KFCWebApp.Models;

namespace KFCWebApp.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this OrderStatus status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? status.ToString();
        }
    }
} 