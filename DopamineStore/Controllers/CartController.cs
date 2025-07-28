using DopamineStore.Data;
using DopamineStore.Models;
using DopamineStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly IViewRenderService _viewRenderService;

        public CartController(ApplicationDbContext context, CartService cartService, IViewRenderService viewRenderService)
        {
            _context = context;
            _cartService = cartService;
            _viewRenderService = viewRenderService;
        }


        public async Task<IActionResult> Index()
        {
            var cartId = _cartService.GetCartId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var cartId = _cartService.GetCartId();
            var cartItem = await _context.CartItems.Include(ci => ci.Product).FirstOrDefaultAsync(ci => ci.Id == id && ci.CartId == cartId);

            if (cartItem == null) return Json(new { success = false, message = "لم يتم العثور على المنتج." });

            bool itemRemoved = false;

            if (quantity > 0 && cartItem.Product.StockQuantity >= quantity)
            {
                cartItem.Quantity = quantity;
            }
            else if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
                itemRemoved = true;
            }
            else if (quantity > cartItem.Product.StockQuantity)
            {
                return Json(new { success = false, message = $"الكمية المتاحة هي {cartItem.Product.StockQuantity} فقط" });
            }

            await _context.SaveChangesAsync();

            var cartItems = await _context.CartItems.Where(c => c.CartId == cartId).Include(c => c.Product).ToListAsync();
            decimal cartTotal = cartItems.Sum(item => item.Quantity * item.Product.Price);
            int cartCount = cartItems.Sum(item => item.Quantity);

            return Json(new
            {
                success = true,
                itemRemoved,
                newQuantity = itemRemoved ? 0 : cartItem.Quantity,
                itemTotal = itemRemoved ? "0" : (cartItem.Quantity * cartItem.Product.Price).ToString("C"),
                cartTotal = cartTotal.ToString("C"),
                cartCount
            });
        }


        // POST: /Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "الكمية غير صالحة." });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.StockQuantity < quantity)
            {
                return Json(new { success = false, message = "الكمية المطلوبة غير متوفرة في المخزون." });
            }

            var cartId = _cartService.GetCartId();
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.CartId == cartId);

            if (cartItem == null)
            {
                cartItem = new CartItem { ProductId = productId, CartId = cartId, Quantity = quantity };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return await GetCartStateAsJson();
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartId = _cartService.GetCartId();
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == id && ci.CartId == cartId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return await GetCartStateAsJson();
        }

        // GET: /Cart/GetCartState (for AJAX updates)
        [HttpGet]
        public async Task<IActionResult> GetCartState()
        {
            return await GetCartStateAsJson();
        }

        private async Task<JsonResult> GetCartStateAsJson()
        {
            var cartId = _cartService.GetCartId();
            var cartItems = await _context.CartItems
                                      .Include(c => c.Product)
                                      .Where(c => c.CartId == cartId)
                                      .ToListAsync();

            decimal total = cartItems.Sum(item => item.Quantity * item.Product.Price);
            int count = cartItems.Sum(item => item.Quantity);

            string cartContentHtml = await _viewRenderService.RenderToStringAsync("_SlideCartPartial", cartItems);

            return Json(new
            {
                success = true,
                cartTotal = total.ToString("C", new CultureInfo("ar-EG")),
                cartCount = count,
                html = cartContentHtml
            });
        }
    }
}
