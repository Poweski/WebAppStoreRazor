using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab12.Pages.Store
{
    public class OrderModel : PageModel
    {
        private readonly StoreDBContext _context;

        public OrderModel(StoreDBContext context)
        {
            _context = context;
        }

        public List<ArticleInCart> Articles { get; set; } = new();

        public void OnGet()
        {
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

        public IActionResult OnPost(string firstName, string lastName, string address, string paymentMethod)
        {
            if (ModelState.IsValid)
            {
                TempData["FirstName"] = firstName;
                TempData["LastName"] = lastName;
                TempData["Address"] = address;
                TempData["PaymentMethod"] = paymentMethod;

                return RedirectToPage("/Store/OrderConfirmation");
            }

            return Page();
        }
    }
}
