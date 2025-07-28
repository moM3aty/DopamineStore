using DopamineStore.Data;
using DopamineStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var wishlistItems = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> AddOrRemove(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "يجب تسجيل الدخول أولاً.", redirectTo = Url.Action("Login", "Account") });
            }

            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);

            bool added;
            if (wishlistItem == null)
            {
                _context.WishlistItems.Add(new WishlistItem { ProductId = productId, UserId = userId });
                added = true;
            }
            else
            {
                _context.WishlistItems.Remove(wishlistItem);
                added = false;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, added });
        }
    }
}
