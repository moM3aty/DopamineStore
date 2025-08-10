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
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // --- Pre-fetch data to reduce database calls ---
            var allOrders = await _context.Orders.ToListAsync();
            var allUsers = await _userManager.Users.ToListAsync();

            // --- Sales Metrics ---
            var totalSales = allOrders.Sum(o => o.TotalAmount);
            var salesToday = allOrders.Where(o => o.OrderDate.Date == today).Sum(o => o.TotalAmount);
            var totalNumberOfOrders = allOrders.Count > 0 ? allOrders.Count : 1;
            var averageOrderValue = totalSales / totalNumberOfOrders;

            // --- Customer Metrics ---
            var firstOrdersThisMonth = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth)
                .GroupBy(o => o.CustomerEmail)
                .Select(g => g.Min(o => o.OrderDate))
                .CountAsync();

            var topCustomers = allOrders
                .GroupBy(o => o.CustomerName)
                .Select(g => new CustomerSpendingViewModel
                {
                    CustomerName = g.Key,
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(5)
                .ToList();

            // --- Product Metrics ---
            var topSellingProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => new { od.ProductId, od.Product.Name, od.Product.ImageUrl })
                .Select(g => new ProductSaleViewModel
                {
                    ProductName = g.Key.Name,
                    ImageUrl = g.Key.ImageUrl,
                    UnitsSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.UnitsSold)
                .Take(5)
                .ToListAsync();

            // --- Chart Data ---
            // Monthly Sales (Last 12 months)
            var monthlySalesData = new decimal[12];
            for (int i = 0; i < 12; i++)
            {
                var month = today.AddMonths(-i);
                monthlySalesData[11 - i] = allOrders
                    .Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month)
                    .Sum(o => o.TotalAmount);
            }

            // Daily Sales (Last 7 Days)
            var last7DaysLabels = new List<string>();
            var dailySalesLast7Days = new List<decimal>();
            for (int i = 0; i < 7; i++)
            {
                var day = today.AddDays(-i);
                last7DaysLabels.Add(day.ToString("ddd", new CultureInfo("ar-EG")));
                dailySalesLast7Days.Add(allOrders.Where(o => o.OrderDate.Date == day).Sum(o => o.TotalAmount));
            }
            last7DaysLabels.Reverse();
            dailySalesLast7Days.Reverse();

            // Top 5 Selling Categories
            var categorySales = await _context.OrderDetails
                .Where(od => od.Product != null && od.Product.Category != null)
                .Include(od => od.Product.Category)
                .GroupBy(od => od.Product.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            // Order Status Distribution
            var orderStatusDistribution = allOrders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Low Stock Products Notification
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

            // --- Assemble the ViewModel ---
            var viewModel = new DashboardViewModel
            {
                TotalSales = totalSales,
                SalesToday = salesToday,
                TotalOrders = allOrders.Count,
                AverageOrderValue = averageOrderValue,
                TotalUsers = allUsers.Count,
                NewCustomersThisMonth = firstOrdersThisMonth,
                TotalProducts = await _context.Products.CountAsync(),
                RecentOrders = allOrders.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
                TopSellingProducts = topSellingProducts,
                TopCustomers = topCustomers,
                MonthlySalesData = monthlySalesData.ToList(),
                CategoryNames = categorySales.Select(cs => cs.CategoryName).ToList(),
                CategorySalesData = categorySales.Select(cs => cs.Count).ToList(),
                DailySalesLast7Days = dailySalesLast7Days,
                Last7DaysLabels = last7DaysLabels,
                OrderStatusLabels = orderStatusDistribution.Select(os => os.Status).ToList(),
                OrderStatusCounts = orderStatusDistribution.Select(os => os.Count).ToList(),
            };

            return View(viewModel);
        }
    }
}
