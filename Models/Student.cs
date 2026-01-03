using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcApp.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessage = "Only letters and spaces allowed")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Roll is required")]
        [RegularExpression(@"^\d+$",
            ErrorMessage = "Roll must be numeric")]
        [StringLength(20)]
        public string? Roll { get; set; }

        public string? PhotoPath { get; set; }

        public int? ClassroomId { get; set; }
        public Classroom? Classroom { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public ParentInfo? ParentInfo { get; set; }

        public ICollection<EnrollmentHistory> EnrollmentHistories { get; set; }
            = new List<EnrollmentHistory>();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
