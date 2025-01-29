using Lab12.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Lab12.Pages.Shared.CategoryMenu
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly StoreDBContext _context;

        public CategoryMenuViewComponent(StoreDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories.ToDictionaryAsync(c => c.Id, c => c.Name);
            return View(categories);
        }
    }
}
