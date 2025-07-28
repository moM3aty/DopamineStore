using DopamineStore.Data;
using DopamineStore.Models;
using DopamineStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var allCategories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            var allProducts = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                Products = allProducts,
                Categories = allCategories,

                Reviews = await _context.Reviews.Where(r => r.IsApproved).OrderByDescending(r => r.ReviewDate).Take(5).ToListAsync(),

                SpecialOffers = await _context.Products
                                        .Where(p => p.OfferEndDate.HasValue && p.OfferEndDate > DateTime.Now)
                                        .Include(p => p.Category)
                                        .OrderBy(p => p.OfferEndDate)
                                        .Take(3)
                                        .ToListAsync(),

                FeaturedProducts = await _context.Products
                                            .Where(p => p.IsFeatured)
                                            .Include(p => p.Category)
                                            .Take(4)
                                            .ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(Review model)
        {
            if (!ModelState.IsValid || model.ProductId <= 0)
            {
                TempData["ErrorMessage"] = "يرجى التأكد من ملء جميع الحقول بشكل صحيح.";
                return RedirectToAction("ProductDetails", new { id = model.ProductId });
            }

            var user = await _userManager.GetUserAsync(User);
            model.ReviewerName = user?.FullName ?? "مستخدم غير مسجل";
            model.ReviewDate = DateTime.Now;
            model.IsApproved = false;

            _context.Reviews.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "شكراً لك! تم إرسال مراجعتك وستظهر بعد الموافقة عليها.";
            return RedirectToAction("ProductDetails", new { id = model.ProductId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendContactMessage(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {
    

                TempData["ContactSuccess"] = "تم إرسال رسالتك بنجاح! شكراً لتواصلك معنا.";
            }
            else
            {
                TempData["ContactError"] = "الرجاء التأكد من ملء جميع الحقول بشكل صحيح.";
            }

            return Redirect(Url.Action("Index", "Home") + "#contact");
        }

    }
}
