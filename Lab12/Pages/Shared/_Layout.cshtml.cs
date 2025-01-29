using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab12.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Pages.Shared.Layout
{
    public class IndexModel : PageModel
    {
        private readonly StoreDBContext _context;

        public IndexModel(StoreDBContext context)
        {
            _context = context;
        }

        public Dictionary<int, string>? Categories { get; set; }
        public int? SelectedCategoryId { get; set; }

        public async Task OnGetAsync(int? categoryId)
        {
            ViewData["Categories"] = await _context.Categories
                .OrderBy(c => c.Name)
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            ViewData["SelectedCategoryId"] = categoryId;

            if (categoryId.HasValue)
            {
                var articles = await _context.Articles
                    .Where(a => a.CategoryId == categoryId)
                    .ToListAsync();

                ViewData["Articles"] = articles;
            }
        }
    }
}
