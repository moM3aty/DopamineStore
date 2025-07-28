using System.ComponentModel.DataAnnotations;

namespace DopamineStore.ViewModels
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string Name { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "الرجاء إدخال بريد إلكتروني صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "الموضوع مطلوب")]
        [Display(Name = "الموضوع")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "الرسالة مطلوبة")]
        [Display(Name = "الرسالة")]
        public string Message { get; set; }
    }
}