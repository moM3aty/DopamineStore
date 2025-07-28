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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userOrders = await _context.Orders
                                     .Where(o => o.CustomerEmail == user.Email)
                                     .OrderByDescending(o => o.OrderDate)
                                     .ToListAsync();
            return View(userOrders);
        }

        // GET: /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                                .Include(o => o.OrderDetails)
                                .ThenInclude(od => od.Product)
                                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.CustomerEmail != user.Email)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
