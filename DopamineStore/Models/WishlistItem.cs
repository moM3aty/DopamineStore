using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
