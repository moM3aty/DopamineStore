using DopamineStore.Models;
using System.Collections.Generic;

namespace DopamineStore.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<decimal> MonthlySalesData { get; set; } = new List<decimal>();
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<int> CategorySalesData { get; set; } = new List<int>();
    }
}