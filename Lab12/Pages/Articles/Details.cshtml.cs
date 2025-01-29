using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Pages.Articles
{
    public class DetailsModel : PageModel
    {
        private readonly StoreDBContext _context;

        public DetailsModel(StoreDBContext context)
        {
            _context = context;
        }

        public Article Article { get; set; }
        public string CategoryName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);

            if (Article == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == Article.CategoryId);
            CategoryName = category?.Name;

            return Page();
        }
    }
}
