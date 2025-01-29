using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab12.Pages.Articles
{
    public class EditModel : PageModel
    {
        private readonly StoreDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(StoreDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Article Article { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public Dictionary<int,String> Categories { get; set; }

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

            ViewData["ImagePath"] = Article.ImagePath;
            ViewData["Categories"] = new Dictionary<int, string>(_context.Categories.ToDictionary(c => c.Id, c => c.Name));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id != Article.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories.ToDictionaryAsync(c => c.Id, c => c.Name);
                return Page();
            }

            try
            {
                var existingArticle = await _context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

                if (existingArticle == null)
                {
                    return NotFound();
                }

                if (string.IsNullOrEmpty(Article.ImagePath))
                {
                    Article.ImagePath = existingArticle.ImagePath;
                }

                _context.Attach(Article).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToPage("/Store/Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Articles.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
