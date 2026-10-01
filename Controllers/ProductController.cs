
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProductStoreMVC.Models;
using ProductStoreMVC.Services;

namespace ProductStoreMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        private readonly IMemoryCache _memoryCache;

        public ProductController(
            ProductService productService,
            IMemoryCache memoryCache)
        {
            _productService = productService;

            _memoryCache = memoryCache;
        }

        // =========================================================
        // 1. PRODUCT LIST + IMEMORYCACHE
        // =========================================================

        public async Task<IActionResult> Index()
        {
            const string cacheKey = "product-list";

            // Check whether product list is already cached
            if (!_memoryCache.TryGetValue(
                cacheKey,
                out List<Product>? products))
            {
                // Get products asynchronously from API
                products =
                    await _productService.GetProductsAsync();

                // Cache for 10 minutes
                var cacheOptions =
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                            TimeSpan.FromMinutes(10));

                _memoryCache.Set(
                    cacheKey,
                    products,
                    cacheOptions);
            }

            return View(products ?? new List<Product>());
        }


        // =========================================================
        // 2. PRODUCT DETAILS + RESPONSE CACHING
        // =========================================================

        [ResponseCache(
            Duration = 60,
            Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Details(int id)
        {
            const string cacheKey = "product-list";

            List<Product>? products;

            // Try to get products from memory cache
            if (!_memoryCache.TryGetValue(
                cacheKey,
                out products))
            {
                products =
                    await _productService.GetProductsAsync();

                var cacheOptions =
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                            TimeSpan.FromMinutes(10));

                _memoryCache.Set(
                    cacheKey,
                    products,
                    cacheOptions);
            }

            // Find product using ProductId
            var product = products?
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // =========================================================
        // 3. COOKIE - STORE CUSTOMER NAME
        // =========================================================

        [HttpPost]
        public IActionResult SetCustomerName(
            string customerName)
        {
            Response.Cookies.Append(
                "CustomerName",
                customerName,
                new CookieOptions
                {
                    Expires =
                        DateTimeOffset.Now.AddDays(7),

                    HttpOnly = true,

                    IsEssential = true
                });

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // 4. SESSION - STORE PRODUCT CATEGORY
        // =========================================================

        [HttpPost]
        public IActionResult SetCategory(
            string category)
        {
            HttpContext.Session.SetString(
                "SelectedCategory",
                category);

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // 5. ORDER PAGE
        // ProductId comes through Query String
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Order(int id)
        {
            const string cacheKey = "product-list";

            List<Product>? products;

            if (!_memoryCache.TryGetValue(
                cacheKey,
                out products))
            {
                products =
                    await _productService.GetProductsAsync();

                var cacheOptions =
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                            TimeSpan.FromMinutes(10));

                _memoryCache.Set(
                    cacheKey,
                    products,
                    cacheOptions);
            }

            var product = products?
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // =========================================================
        // 6. SUBMIT ORDER
        // ProductId comes from Hidden Field
        // =========================================================

        [HttpPost]
        public IActionResult Order(
            int productId,
            string customerName)
        {
            TempData["Message"] =
                $"Order submitted successfully for Product ID: {productId}.";

            return RedirectToAction(nameof(Index));
        }
    }
}