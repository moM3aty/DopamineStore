using DopamineStore.Models;
using System.Collections.Generic;

namespace DopamineStore.ViewModels
{
    /// <summary>
    /// Represents the data required for the product details page,
    /// including the main product and a list of related products.
    /// </summary>
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}
