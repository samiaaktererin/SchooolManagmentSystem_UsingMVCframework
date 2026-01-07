// file: Models/TeacherSubject.cs
using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class TeacherSubject
    {
        public int Id { get; set; }

        // FK to Teacher
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        // Optional FK to Subject entity (nullable so existing code won't break)
        public int? SubjectId { get; set; }

        public Subject? Subject { get; set; }

        // Fallback: store subject name directly
        [Required]
        public string SubjectName { get; set; } = string.Empty;

        // Optionally associate subject with a classroom/grade
        public int? ClassroomId { get; set; }
        public Classroom? Classroom { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }
        


    }
}
