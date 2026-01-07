using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        // ---------------- Name ----------------
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        [RegularExpression(@"^[A-Za-z\s]+$",
            ErrorMessage = "Name can contain only letters and spaces")]
        public string Name { get; set; } = string.Empty;

        // ---------------- Email ----------------
        [Required(ErrorMessage = "Email is required")]
        [StringLength(200)]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string? Email { get; set; }

        // ---------------- Phone ----------------
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(50)]
        [RegularExpression(@"^[0-9]+$",
            ErrorMessage = "Phone number can contain only digits")]
        public string? Phone { get; set; }

        // ---------------- Status ----------------
        public bool IsActive { get; set; } = true;

        // ---------------- Class Assignment ----------------
        public int? ClassAssignedId { get; set; }
        public Classroom? ClassAssigned { get; set; }

        // ---------------- Photo ----------------
        public string? PhotoPath { get; set; }

        // ---------------- Relations ----------------
        public ICollection<TeacherAttendance> Attendances { get; set; } = new List<TeacherAttendance>();
        public ICollection<TeacherSalary> Salaries { get; set; } = new List<TeacherSalary>();
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    }
}
