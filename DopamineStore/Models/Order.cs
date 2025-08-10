using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "اسم العميل مطلوب")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني للعميل مطلوب")]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }

        [Display(Name = "كود الخصم المطبق")]
        public string? AppliedCouponCode { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "قيمة الخصم")]
        public decimal DiscountAmount { get; set; } = 0;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
