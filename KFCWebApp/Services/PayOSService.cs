using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace KFCWebApp.Services
{
    public class PayOSService
    {
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly HttpClient _httpClient;

        public PayOSService(IConfiguration config)
        {
            _clientId = config["PayOS:ClientId"];
            _apiKey = config["PayOS:ApiKey"];
            _checksumKey = config["PayOS:ChecksumKey"];
            _httpClient = new HttpClient();
        }

        public async Task<string?> CreatePaymentLink(decimal amount, string orderId, string returnUrl, string description)
        {
            var url = "https://api-merchant.payos.vn/v2/payment-requests";
            var intAmount = (int)amount;
            // Tạo signature
            string rawData = $"amount={intAmount}&cancelUrl={returnUrl}&description={description}&orderCode={orderId}&returnUrl={returnUrl}";
            var keyBytes = Encoding.UTF8.GetBytes(_checksumKey);
            var dataBytes = Encoding.UTF8.GetBytes(rawData);
            string signature;
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            var payload = new
            {
                orderCode = int.Parse(orderId),
                amount = intAmount,
                description = description,
                returnUrl = returnUrl,
                cancelUrl = returnUrl,
                buyerName = "Khach hang",
                buyerEmail = "",
                buyerPhone = "",
                signature = signature
            };
            var json = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-client-id", _clientId);
            request.Headers.Add("x-api-key", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            var resJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resJson);
            var payUrl = doc.RootElement.GetProperty("data").GetProperty("checkoutUrl").GetString();
            return payUrl;
        }

        public async Task<string> GetPaymentStatus(string orderId)
        {
            try
            {
                var url = $"https://api-merchant.payos.vn/v2/payment-requests/{orderId}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("x-client-id", _clientId);
                request.Headers.Add("x-api-key", _apiKey);
                
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return "UNKNOWN";
                
                var resJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(resJson);
                var status = doc.RootElement.GetProperty("data").GetProperty("status").GetString();
                return status ?? "UNKNOWN";
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
} 