using DopamineStore.Models;
using System.Collections.Generic;

namespace DopamineStore.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Product> LatestProducts { get; set; } = new List<Product>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Product> SpecialOffers { get; set; } = new List<Product>();
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
    }
}
