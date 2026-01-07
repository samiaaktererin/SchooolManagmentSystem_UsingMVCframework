using static System.Collections.Specialized.BitVector32;

namespace MyMvcApp.Models
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // e.g. "Grade 6"
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
