using System.Text.Json;
using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Pages.Store
{
    [Authorize(Policy = "CanAccessCart")]
    public class CartModel : PageModel
    {
        private readonly StoreDBContext _context;

        public CartModel(StoreDBContext context)
        {
            _context = context;
        }

        public List<ArticleInCart> Articles { get; set; } = new();

        public Dictionary<int, string> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? articleId, int? quantity)
        {
            Categories = await _context.Categories
                       .OrderBy(c => c.Name)
                       .ToDictionaryAsync(c => c.Id, c => c.Name);

            ViewData["Categories"] = Categories;

            var cart = new Cart { Items = new List<ArticleInCart>() };

            foreach (var cookie in Request.Cookies)
            {
                if (cookie.Key.StartsWith("article"))
                {
                    int articleIdFromCookie = int.Parse(cookie.Key.Substring(7));
                    int quantityInCart = int.Parse(cookie.Value);

                    var articleInCart = new ArticleInCart
                    {
                        ArticleId = articleIdFromCookie,
                        Quantity = quantityInCart
                    };

                    cart.Items.Add(articleInCart);
                }
            }

            if (articleId.HasValue)
            {
                if (quantity.HasValue)
                {
                    if (quantity.Value <= 0)
                    {
                        cart.Items.RemoveAll(a => a.ArticleId == articleId.Value);
                        Response.Cookies.Delete("article" + articleId.Value);
                    }
                    else
                    {
                        var articleInCart = cart.Items.FirstOrDefault(a => a.ArticleId == articleId.Value);
                        if (articleInCart != null)
                        {
                            articleInCart.Quantity = quantity.Value;
                        }
                        else
                        {
                            cart.Items.Add(new ArticleInCart { ArticleId = articleId.Value, Quantity = quantity.Value });
                        }

                        Response.Cookies.Append("article" + articleId.Value, quantity.Value.ToString(), new CookieOptions
                        {
                            HttpOnly = false,
                            Expires = DateTimeOffset.Now.AddDays(7),
                            SameSite = SameSiteMode.Lax
                        });
                    }
                }
                else
                {
                    cart.Items.RemoveAll(a => a.ArticleId == articleId.Value);
                    Response.Cookies.Delete("article" + articleId.Value);
                }
            }

            var articleIds = cart.Items.Select(i => i.ArticleId).ToList();
            var articlesFromDb = await _context.Articles
                .Where(a => articleIds.Contains(a.Id))
                .ToListAsync();

            Articles = cart.Items
                .Join(articlesFromDb,
                      cartItem => cartItem.ArticleId,
                      dbArticle => dbArticle.Id,
                      (cartItem, dbArticle) => new ArticleInCart
                      {
                          ArticleId = dbArticle.Id,
                          Name = dbArticle.Name,
                          Quantity = cartItem.Quantity,
                          Price = dbArticle.Price,
                          Total = dbArticle.Price * cartItem.Quantity,
                          ImagePath = dbArticle.ImagePath,
                          CategoryId = dbArticle.CategoryId
                      })
                .ToList();

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

        public IActionResult OnGetUpdateCart(int articleId, int quantity)
        {            
            var cartJson = Request.Cookies["Cart"];
            var cart = string.IsNullOrEmpty(cartJson)
                ? new Cart()
                : JsonSerializer.Deserialize<Cart>(cartJson);

            var productExists = _context.Articles.Any(p => p.Id == articleId);

            if (quantity <= 0 || !productExists)
            {
                return OnGetRemoveFromCart(articleId);
            }

            if (cart?.Items == null)
            {
                cart.Items = new List<ArticleInCart>();
            }

            var articleInCart = cart.Items.FirstOrDefault(a => a.ArticleId == articleId);
            if (articleInCart != null)
            {
                articleInCart.Quantity = quantity;
            }
            else
            {
                cart.Items.Add(new ArticleInCart { ArticleId = articleId, Quantity = quantity });
            }

            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                SameSite = SameSiteMode.Lax,
                HttpOnly = false
            };
            Response.Cookies.Append("article" + articleId, quantity.ToString(), options);

            var newCartJson = JsonSerializer.Serialize(cart);
            Response.Cookies.Append("Cart", newCartJson, options);

            return RedirectToPage();
        }
    }
}
