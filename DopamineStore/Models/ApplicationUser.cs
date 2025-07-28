using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DopamineStore.Models
{

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
