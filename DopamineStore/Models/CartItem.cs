using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
