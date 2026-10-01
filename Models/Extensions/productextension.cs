using ProductStoreMVC.Models;

namespace ProductStoreMVC.Extensions
{
    public static class ProductExtensions
    {
        public static decimal GetDiscountedPrice(this Product product)
        {
            decimal discountAmount =
                product.Price * product.DiscountPercentage / 100;

            return product.Price - discountAmount;
        }
    }
}