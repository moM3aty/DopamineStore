using DopamineStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DopamineStore.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [Display(Name = "العنوان بالتفصيل")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار منطقة الشحن")]
        [Display(Name = "اختر منطقتك")]
        public int ShippingZoneId { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار طريقة الدفع")]
        [Display(Name = "طريقة الدفع")]
        public string PaymentMethod { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public SelectList ShippingZones { get; set; }
    }
}