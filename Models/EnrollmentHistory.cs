namespace MyMvcApp.Models
{
    public class EnrollmentHistory
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;   // ← MUST BE HERE

        public int ClassroomId { get; set; }
        public Classroom Classroom { get; set; } = null!;

        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
        public string? Note { get; set; }
    }
}
