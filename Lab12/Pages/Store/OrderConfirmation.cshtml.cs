using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab12.Pages.Store
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly StoreDBContext _context;

        public OrderConfirmationModel(StoreDBContext context)
        {
            _context = context;
        }

        public List<ArticleInCart> Articles { get; set; } = new();

        public string FirstName { get; set; } = "Unknown";
        public string LastName { get; set; } = "Unknown";
        public string Address { get; set; } = "Unknown Address";
        public string PaymentMethod { get; set; } = "Unknown";

        public decimal TotalAmount => Articles.Sum(a => a.Total);

        public void OnGet()
        {
            FirstName = TempData["FirstName"]?.ToString() ?? "Unknown";
            LastName = TempData["LastName"]?.ToString() ?? "Unknown";
            Address = TempData["Address"]?.ToString() ?? "Unknown Address";
            PaymentMethod = TempData["PaymentMethod"]?.ToString() ?? "Unknown";

            var cart = new Cart { Items = new List<ArticleInCart>() };

            foreach (var cookie in Request.Cookies)
            {
                if (cookie.Key.StartsWith("article"))
                {
                    int articleId = int.Parse(cookie.Key.Substring(7));
                    int quantity = int.Parse(cookie.Value);

                    cart.Items.Add(new ArticleInCart
                    {
                        ArticleId = articleId,
                        Quantity = quantity
                    });
                }
            }

            var articleIds = cart.Items.Select(i => i.ArticleId).ToList();
            var articlesFromDb = _context.Articles.Where(a => articleIds.Contains(a.Id)).ToList();

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
                          ImagePath = dbArticle.ImagePath
                      })
                .ToList();
        }

        public IActionResult OnPost()
        {
            foreach (var cookie in Request.Cookies)
            {
                if (cookie.Key.StartsWith("article"))
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToPage("/Store/Index");
        }
    }
}
