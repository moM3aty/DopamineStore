using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(200)]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; }

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "سعر المنتج مطلوب")]
        [Column(TypeName = "decimal(18, 0)")]
        [Range(0.01, 1000000, ErrorMessage = "الرجاء إدخال سعر صحيح")]
        [Display(Name = "السعر الحالي")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        [Display(Name = "السعر القديم (قبل الخصم)")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "صورة المنتج الرئيسية")]
        public string? ImageUrl { get; set; }

        [Display(Name = "الكمية المتاحة")]
        [Range(0, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون رقمًا موجبًا")]
        public int StockQuantity { get; set; } = 0;

        [Display(Name = "تاريخ الإضافة")]
        public DateTime EntryDate { get; set; } = DateTime.Now;

        [Display(Name = "القسم")]
        [Required(ErrorMessage = "يجب اختيار قسم للمنتج")]
        public int CategoryId { get; set; }
        [Display(Name = "منتج مميز؟")]
        public bool IsFeatured { get; set; } = false;
        [Display(Name = "تاريخ انتهاء العرض")]
        public DateTime? OfferEndDate { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<ProductImg> ProductImages { get; set; } = new List<ProductImg>();
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
