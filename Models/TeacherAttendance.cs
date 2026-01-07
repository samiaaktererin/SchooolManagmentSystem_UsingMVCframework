using System;

namespace MyMvcApp.Models
{
    public class TeacherAttendance
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public bool Present { get; set; } = true;
        public string? Note { get; set; }
    }
}
