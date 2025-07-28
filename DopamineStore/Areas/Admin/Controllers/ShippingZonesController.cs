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
    public class ShippingZonesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShippingZonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ShippingZones
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShippingZones.OrderBy(z => z.Name).ToListAsync());
        }

        // GET: Admin/ShippingZones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ShippingZones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Cost")] ShippingZone shippingZone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shippingZone);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تمت إضافة منطقة الشحن بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            return View(shippingZone);
        }

        // GET: Admin/ShippingZones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingZone = await _context.ShippingZones.FindAsync(id);
            if (shippingZone == null)
            {
                return NotFound();
            }
            return View(shippingZone);
        }

        // POST: Admin/ShippingZones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Cost")] ShippingZone shippingZone)
        {
            if (id != shippingZone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippingZone);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "تم تعديل منطقة الشحن بنجاح.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ShippingZones.Any(e => e.Id == shippingZone.Id))
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
            return View(shippingZone);
        }

        // GET: Admin/ShippingZones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingZone = await _context.ShippingZones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingZone == null)
            {
                return NotFound();
            }

            return View(shippingZone);
        }

        // POST: Admin/ShippingZones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shippingZone = await _context.ShippingZones.FindAsync(id);
            _context.ShippingZones.Remove(shippingZone);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف منطقة الشحن بنجاح.";
            return RedirectToAction(nameof(Index));
        }
    }
}
