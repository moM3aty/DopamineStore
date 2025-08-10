using DopamineStore.Models;
using System.Collections.Generic;

namespace DopamineStore.ViewModels
{

    public class DashboardViewModel
    {
        public decimal TotalSales { get; set; }
        public decimal SalesToday { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalUsers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int TotalProducts { get; set; }

        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<ProductSaleViewModel> TopSellingProducts { get; set; } = new List<ProductSaleViewModel>();
        public List<CustomerSpendingViewModel> TopCustomers { get; set; } = new List<CustomerSpendingViewModel>();

        public List<decimal> MonthlySalesData { get; set; } = new List<decimal>();
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<int> CategorySalesData { get; set; } = new List<int>();
        public List<decimal> DailySalesLast7Days { get; set; } = new List<decimal>();
        public List<string> Last7DaysLabels { get; set; } = new List<string>();
        public List<string> OrderStatusLabels { get; set; } = new List<string>();
        public List<int> OrderStatusCounts { get; set; } = new List<int>();
    }

    public class ProductSaleViewModel
    {
        public string ProductName { get; set; }
        public int UnitsSold { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CustomerSpendingViewModel
    {
        public string CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
