using DopamineStore.Data;
using DopamineStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var categoriesQuery = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                categoriesQuery = categoriesQuery.Where(c => c.Name.Contains(searchString));
            }

            var categories = await categoriesQuery.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View(new Category());
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
                {
                    ModelState.AddModelError("Name", "اسم القسم موجود بالفعل.");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم إضافة القسم بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != id))
                {
                    ModelState.AddModelError("Name", "اسم القسم موجود بالفعل.");
                    return View(category);
                }

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "تم تعديل القسم بنجاح.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم حذف القسم بنجاح.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Categories/CreateFromModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromModal([FromBody] Category model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { success = false, message = "اسم القسم مطلوب." });
            }

            if (await _context.Categories.AnyAsync(c => c.Name == model.Name))
            {
                return Json(new { success = false, message = "اسم القسم موجود بالفعل." });
            }

            var newCategory = new Category { Name = model.Name };
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return Json(new { success = true, id = newCategory.Id, name = newCategory.Name });
        }
    }
}
