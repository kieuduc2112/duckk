using System.Text.Json.Serialization;

namespace KFCWebApp.Models
{
    public class PayOSWebhookData
    {
        [JsonPropertyName("orderId")]
        public string orderId { get; set; } = string.Empty;
        
        [JsonPropertyName("status")]
        public string status { get; set; } = string.Empty;
        
        [JsonPropertyName("amount")]
        public decimal amount { get; set; }
        
        [JsonPropertyName("transactionId")]
        public string transactionId { get; set; } = string.Empty;
        
        [JsonPropertyName("signature")]
        public string signature { get; set; } = string.Empty;
    }
} 