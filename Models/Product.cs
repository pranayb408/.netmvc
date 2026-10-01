namespace ProductStoreMVC.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        // Price is stored in INR
        public decimal Price { get; set; }

        public decimal DiscountPercentage { get; set; }
    }
}