namespace MyMvcApp.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // e.g. "A"
        public int ClassroomId { get; set; }
        public Classroom Classroom { get; set; } = null!;
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
