namespace MyMvcApp.Models.ViewModels
{
    public class TeacherDashboardRowVM
    {
        public string TeacherName { get; set; } = "";
        public bool IsActive { get; set; }
        public bool IsPresentToday { get; set; }
    }
}
