using System;

namespace MyMvcApp.Models
{
    public class TeacherSalary
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // store the month as DateTime? (use first day of month)
        public DateTime? SalaryMonth { get; set; }

        public decimal Amount { get; set; }

        // string for method (Cash/Bank/etc.)
        public string? PaymentMethod { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }
    }
}
