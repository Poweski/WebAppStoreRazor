using System.Text.Json;
using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Pages.Store
{
    [Route("Store")]
    [Route("Store/Index")]
    public class IndexModel : PageModel
    {
        private readonly StoreDBContext _context;

        public IndexModel(StoreDBContext context)
        {
            _context = context;
        }

        public List<Article> Articles { get; set; } = new();
        public Dictionary<int, string> Categories { get; set; } = new();

        public async Task<IActionResult> OnGet(int? categoryId)
        {
            Categories = await _context.Categories
                       .OrderBy(c => c.Name)
                       .ToDictionaryAsync(c => c.Id, c => c.Name);

            ViewData["Categories"] = Categories;
            ViewData["SelectedCategoryId"] = categoryId;

            Articles = await _context.Articles
                .Where(a => !categoryId.HasValue || a.CategoryId == categoryId)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return Page();
        }

        public IActionResult OnGetRemoveFromCart(int articleId)
        {
            var cartJson = Request.Cookies["Cart"];
            var cart = string.IsNullOrEmpty(cartJson)
                ? new Cart()
                : JsonSerializer.Deserialize<Cart>(cartJson);

            if (cart?.Items != null)
            {
                cart.Items.RemoveAll(i => i.ArticleId == articleId);

                Response.Cookies.Delete("article" + articleId);

                var newCartJson = JsonSerializer.Serialize(cart);
                Response.Cookies.Append("Cart", newCartJson, new CookieOptions
                {
                    HttpOnly = false,
                    Expires = DateTimeOffset.Now.AddDays(7),
                    SameSite = SameSiteMode.Lax
                });
            }

            return RedirectToPage();
        }

        public IActionResult OnPostAddToCart(int articleId)
        {
            Console.WriteLine("ARTICLE ID: " + articleId);
            var productExists = _context.Articles.Any(p => p.Id == articleId);

            if (!productExists)
            {
                Console.WriteLine($"Product with ID {articleId} does not exist.");
                return OnGetRemoveFromCart(articleId);
            }

            var cartJson = Request.Cookies["Cart"];
            var cart = string.IsNullOrEmpty(cartJson)
                ? new Cart()
                : JsonSerializer.Deserialize<Cart>(cartJson);

            if (cart.Items == null)
            {
                cart.Items = new List<ArticleInCart>();
            }

            var articleCookie = Request.Cookies["article" + articleId];
            int quantity = string.IsNullOrEmpty(articleCookie) ? 0 : int.Parse(articleCookie);

            if (quantity > 0)
            {
                var articleInCart = cart.Items.FirstOrDefault(a => a.ArticleId == articleId);
                if (articleInCart != null)
                {
                    articleInCart.Quantity++;
                    Console.WriteLine($"Increased quantity of article {articleId} to {articleInCart.Quantity}");
                }
                else
                {
                    articleInCart = new ArticleInCart { ArticleId = articleId, Quantity = 1 };
                    cart.Items.Add(articleInCart);
                    Console.WriteLine($"Added new article {articleId} to the cart");
                }
            }
            else
            {
                var articleInCart = new ArticleInCart { ArticleId = articleId, Quantity = 1 };
                cart.Items.Add(articleInCart);
            }

            quantity++;
            Response.Cookies.Append("article" + articleId, quantity.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                SameSite = SameSiteMode.Lax,
                HttpOnly = false
            });

            var newCartJson = JsonSerializer.Serialize(cart);
            Response.Cookies.Append("Cart", newCartJson, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                SameSite = SameSiteMode.Lax,
                HttpOnly = false
            });

            return RedirectToPage();
        }
    }
}
