using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab12.Pages.Articles
{
    public class CreateModel : PageModel
    {
        private readonly StoreDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(StoreDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Article Article { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public SelectList Categories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Categories"] = new Dictionary<int, string>(_context.Categories.ToDictionary(c => c.Id, c => c.Name));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "upload");

                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    var filePath = Path.Combine(uploadDirectory, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    Article.ImagePath = "/upload/" + fileName;
                }

                _context.Articles.Add(Article);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Store/Index");
            }

            Categories = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }
    }
}
