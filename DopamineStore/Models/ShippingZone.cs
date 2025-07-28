using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public class ShippingZone
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنطقة مطلوب")]
        [Display(Name = "اسم المنطقة")]
        public string Name { get; set; }

        [Required(ErrorMessage = "تكلفة الشحن مطلوبة")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "تكلفة الشحن")]
        public decimal Cost { get; set; }
    }
}
