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
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? rating)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentRating"] = rating;
            var reviewsQuery = _context.Reviews.Include(r => r.Product).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                reviewsQuery = reviewsQuery.Where(r => r.Product.Name.Contains(searchString)
                                                  || r.ReviewerName.Contains(searchString)
                                                  || r.Comment.Contains(searchString));
            }

            if (rating.HasValue)
            {
                reviewsQuery = reviewsQuery.Where(r => r.Rating == rating.Value);
            }

            var reviews = await reviewsQuery.OrderByDescending(r => r.ReviewDate).ToListAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleApproval(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return Json(new { success = false, message = "لم يتم العثور على المراجعة." });
            }

            review.IsApproved = !review.IsApproved;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isApproved = review.IsApproved, newLabel = review.IsApproved ? "معتمد" : "غير معتمد" });
        }

        // GET: Admin/Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var review = await _context.Reviews.Include(r => r.Product).FirstOrDefaultAsync(m => m.Id == id);
            if (review == null) return NotFound();
            return View(review);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم حذف المراجعة بنجاح.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
