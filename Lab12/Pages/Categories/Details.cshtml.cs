using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab12.Data;
using Lab12.Models;

namespace Lab12.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly StoreDBContext _context;

        public DetailsModel(StoreDBContext context)
        {
            _context = context;
        }

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
    }
}
