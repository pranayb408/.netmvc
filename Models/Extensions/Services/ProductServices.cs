using System.Text.Json;
using ProductStoreMVC.Models;

namespace ProductStoreMVC.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        // USD to INR conversion rate for this practical
        private const decimal UsdToInr = 96.17m;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    "https://fakestoreapi.com/products");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var apiProducts =
                    JsonSerializer.Deserialize<List<ApiProduct>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (apiProducts == null)
                {
                    return new List<Product>();
                }

                return apiProducts.Select(p => new Product
                {
                    ProductId = p.Id,

                    ProductName = p.Title,

                    Category = p.Category,

                    // Convert USD price to INR
                    Price = p.Price * UsdToInr,

                    // 10% discount for practical demonstration
                    DiscountPercentage = 10
                }).ToList();
            }
            catch (Exception)
            {
                return new List<Product>();
            }
        }
    }

    // Model used to receive Fake Store API data
    public class ApiProduct
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Category { get; set; } = string.Empty;
    }
}