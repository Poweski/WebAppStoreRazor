using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Pages.Articles
{
    public class DeleteModel : PageModel
    {
        private readonly StoreDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeleteModel(StoreDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Article Article { get; set; }

        public string CategoryName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article = await _context.Articles.FirstOrDefaultAsync(m => m.Id == id);

            if (Article == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == Article.CategoryId);
            CategoryName = category?.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                if (!string.IsNullOrEmpty(article.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, article.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Store/Index");
        }
    }
}
