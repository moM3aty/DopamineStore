using DopamineStore.Data;
using DopamineStore.Models;
using DopamineStore.Services;
using DopamineStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, CartService cartService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartId = _cartService.GetCartId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                SubTotal = cartItems.Sum(item => item.Quantity * item.Product.Price),
                ShippingZones = new SelectList(await _context.ShippingZones.ToListAsync(), "Id", "Name")
            };

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                viewModel.CustomerName = user.FullName;
                viewModel.CustomerEmail = user.Email;
                viewModel.PhoneNumber = user.PhoneNumber;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var cartId = _cartService.GetCartId();
            model.CartItems = await _context.CartItems.Include(c => c.Product).Where(c => c.CartId == cartId).ToListAsync();

            if (!model.CartItems.Any()) return RedirectToAction("Index", "Cart");

            model.SubTotal = model.CartItems.Sum(item => item.Quantity * item.Product.Price);

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.CustomerEmail = user.Email;
                ModelState.Remove("CustomerEmail");
            }

            ModelState.Remove(nameof(model.ShippingZones));
            ModelState.Remove(nameof(model.CartItems));

            if (!ModelState.IsValid)
            {
                model.ShippingZones = new SelectList(await _context.ShippingZones.ToListAsync(), "Id", "Name", model.ShippingZoneId);
                return View("Index", model);
            }

            var shippingZone = await _context.ShippingZones.FindAsync(model.ShippingZoneId);
            if (shippingZone == null)
            {
                ModelState.AddModelError("ShippingZoneId", "منطقة الشحن غير صالحة.");
                model.ShippingZones = new SelectList(await _context.ShippingZones.ToListAsync(), "Id", "Name", model.ShippingZoneId);
                return View("Index", model);
            }

            decimal discountAmount = 0;
            if (!string.IsNullOrEmpty(model.CouponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == model.CouponCode.ToUpper());
                if (coupon != null && coupon.IsActive && coupon.ExpiryDate >= DateTime.Today)
                {
                    discountAmount = CalculateDiscount(model.SubTotal, coupon);
                }
            }

            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = model.ShippingAddress,
                City = shippingZone.Name,
                PaymentMethod = model.PaymentMethod,
                OrderStatus = "قيد الانتظار",
                AppliedCouponCode = model.CouponCode,
                DiscountAmount = discountAmount,
                TotalAmount = (model.SubTotal - discountAmount) + shippingZone.Cost
            };

            foreach (var item in model.CartItems)
            {
                order.OrderDetails.Add(new OrderDetail { ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = item.Product.Price });
                var productInDb = await _context.Products.FindAsync(item.ProductId);
                if (productInDb != null) productInDb.StockQuantity -= item.Quantity;
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(model.CartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var cartId = _cartService.GetCartId();
            var cartItems = await _context.CartItems.Include(c => c.Product).Where(c => c.CartId == cartId).ToListAsync();
            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "سلة التسوق فارغة." });
            }

            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper());

            if (coupon == null || !coupon.IsActive || coupon.ExpiryDate < DateTime.Today)
            {
                return Json(new { success = false, message = "كود الخصم غير صالح أو منتهي الصلاحية." });
            }

            var subtotal = cartItems.Sum(item => item.Quantity * item.Product.Price);
            var discountAmount = CalculateDiscount(subtotal, coupon);

            return Json(new { success = true, message = "تم تطبيق الخصم بنجاح!", discountAmount });
        }

        private decimal CalculateDiscount(decimal subtotal, Coupon coupon)
        {
            if (coupon.DiscountType == DiscountType.Percentage)
            {
                return (subtotal * coupon.DiscountValue) / 100;
            }
            return Math.Min(subtotal, coupon.DiscountValue); 
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetShippingCost(int zoneId)
        {
            var shippingZone = await _context.ShippingZones.FindAsync(zoneId);
            if (shippingZone == null)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true, cost = shippingZone.Cost });
        }
    }
}
