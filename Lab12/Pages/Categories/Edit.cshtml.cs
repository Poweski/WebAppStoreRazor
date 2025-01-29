using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab12.Data;
using Lab12.Models;

namespace Lab12.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly StoreDBContext _context;

        public EditModel(StoreDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _context.Categories.FindAsync(id);

            if (Category == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var categoryToUpdate = await _context.Categories.FindAsync(id);

            if (categoryToUpdate == null)
            {
                return NotFound();
            }

            categoryToUpdate.Name = Category.Name;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
