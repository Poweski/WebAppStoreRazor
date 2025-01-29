using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab12.Data;
using Lab12.Models;

namespace Lab12.Pages.Categories
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
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (Category == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var categoryToDelete = await _context.Categories.FindAsync(id);

            if (categoryToDelete == null)
            {
                return NotFound();
            }

            var articles = _context.Articles.Where(a => a.CategoryId == id);

            foreach (var article in articles)
            {
                if (!string.IsNullOrEmpty(article.ImagePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, article.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Articles.Remove(article);
            }

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
