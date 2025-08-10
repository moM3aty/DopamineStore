using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public enum DiscountType
    {
        [Display(Name = "مبلغ ثابت")]
        FixedAmount,
        [Display(Name = "نسبة مئوية")]
        Percentage
    }

    public class Coupon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "كود الكوبون مطلوب")]
        [Display(Name = "كود الكوبون")]
        public string Code { get; set; }

        [Required(ErrorMessage = "نوع الخصم مطلوب")]
        [Display(Name = "نوع الخصم")]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "قيمة الخصم مطلوبة")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "قيمة الخصم")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "تاريخ انتهاء الصلاحية مطلوب")]
        [Display(Name = "تاريخ انتهاء الصلاحية")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; } = true;
    }
}
