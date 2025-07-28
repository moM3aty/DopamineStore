using DopamineStore.Data;
using DopamineStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, int page = 1)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            int pageSize = 9;
            var totalItems = await productsQuery.CountAsync();
            var products = await productsQuery
                                 .OrderByDescending(p => p.EntryDate)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            var viewModel = new ShopViewModel
            {
                Products = products,
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                CurrentSearch = searchString,
                CurrentCategory = categoryId,
                CurrentPage = page,
                TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize)
            };

            return View(viewModel);
        }
    }
}
