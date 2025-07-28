using DopamineStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, string stockLevel)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentStockLevel"] = stockLevel;

            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(stockLevel))
            {
                switch (stockLevel)
                {
                    case "in_stock":
                        productsQuery = productsQuery.Where(p => p.StockQuantity > 5);
                        break;
                    case "low_stock":
                        productsQuery = productsQuery.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 5);
                        break;
                    case "out_of_stock":
                        productsQuery = productsQuery.Where(p => p.StockQuantity == 0);
                        break;
                }
            }

            var sortedProducts = await productsQuery
                .OrderBy(p => p.StockQuantity == 0 ? 0 : (p.StockQuantity <= 5 ? 1 : 2))
                .ThenBy(p => p.StockQuantity)
                .ToListAsync();

            ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            return View(sortedProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int productId, int quantity)
        {
            if (quantity < 0)
            {
                return Json(new { success = false, message = "الكمية لا يمكن أن تكون سالبة." });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "لم يتم العثور على المنتج." });
            }

            product.StockQuantity = quantity;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تم تحديث المخزون بنجاح." });
        }
    }
}
