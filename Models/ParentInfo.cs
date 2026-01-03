using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class ParentInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Father name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters allowed")]
        public string FatherName { get; set; } = null!;

        [Required(ErrorMessage = "Father phone is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers allowed")]
        [StringLength(15, MinimumLength = 6)]
        public string FatherPhone { get; set; } = null!;

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}
