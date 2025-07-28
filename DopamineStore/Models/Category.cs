using System.ComponentModel.DataAnnotations;

namespace DopamineStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب")]
        [Display(Name = "اسم القسم")]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}