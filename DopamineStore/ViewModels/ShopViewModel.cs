using DopamineStore.Models;
using System.Collections.Generic;

namespace DopamineStore.ViewModels
{
    public class ShopViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string CurrentSearch { get; set; }
        public int? CurrentCategory { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
