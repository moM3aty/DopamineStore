using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineStore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "اسم المراجع مطلوب")]
        public string ReviewerName { get; set; }

        [Required(ErrorMessage = "التعليق مطلوب")]
        public string Comment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
