using DopamineStore.Data;
using DopamineStore.Models;
using DopamineStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Monthly Sales Data (Last 12 months)
            var monthlySalesData = new decimal[12];
            var today = DateTime.UtcNow;
            for (int i = 0; i < 12; i++)
            {
                var month = today.AddMonths(-i);
                var total = await _context.Orders
                    .Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month)
                    .SumAsync(o => o.TotalAmount);
                monthlySalesData[11 - i] = total;
            }

            // Top 5 Selling Categories
            var categorySales = await _context.OrderDetails
                .Include(od => od.Product.Category)
                .GroupBy(od => od.Product.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            // Low Stock Products for Notification
            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity > 0 && p.StockQuantity <= 5)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            if (lowStockProducts.Any())
            {
                var warningMessage = "المنتجات التالية مخزونها منخفض: <br/><ul>";
                foreach (var product in lowStockProducts)
                {
                    warningMessage += $"<li>{product.Name} (المتبقي: {product.StockQuantity})</li>";
                }
                warningMessage += "</ul>";
                TempData["LowStockWarning"] = warningMessage;
            }


            var viewModel = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentOrders = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).OrderByDescending(o => o.OrderDate).Take(5).ToListAsync(),
                MonthlySalesData = monthlySalesData.ToList(),
                CategoryNames = categorySales.Select(cs => cs.CategoryName).ToList(),
                CategorySalesData = categorySales.Select(cs => cs.Count).ToList()
            };

            return View(viewModel);
        }
    }
}
