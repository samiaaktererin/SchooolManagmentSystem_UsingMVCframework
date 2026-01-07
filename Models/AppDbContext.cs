using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MyMvcApp.Models
{
    public class AppDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>


    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Existing domain
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Classroom> Classrooms { get; set; } = null!;
        public DbSet<Section> Sections { get; set; } = null!;
        public DbSet<ParentInfo> Parents { get; set; } = null!;
        public DbSet<EnrollmentHistory> EnrollmentHistories { get; set; } = null!;

        // Teacher domain
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<TeacherAttendance> TeacherAttendances { get; set; } = null!;
        public DbSet<TeacherSalary> TeacherSalaries { get; set; } = null!;
       

        // <-- NEW
        public DbSet<TeacherSubject> TeacherSubjects { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------- Student / Parent / Section / Enrollment configuration ----------------

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .HasMaxLength(200);

                entity.Property(e => e.Roll)
                    .HasMaxLength(20);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(6)");

                entity.HasOne(s => s.ParentInfo)
                    .WithOne(p => p.Student)
                    .HasForeignKey<ParentInfo>(p => p.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Classroom)
                    .WithMany()
                    .HasForeignKey(s => s.ClassroomId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.Section)
                    .WithMany(sec => sec.Students)
                    .HasForeignKey(s => s.SectionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ParentInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Classroom>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.HasOne(s => s.Classroom)
                    .WithMany(c => c.Sections)
                    .HasForeignKey(s => s.ClassroomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EnrollmentHistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EnrolledAt).HasColumnType("datetime(6)");
                entity.Property(e => e.LeftAt).HasColumnType("datetime(6)");

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.EnrollmentHistories)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Classroom)
                    .WithMany()
                    .HasForeignKey(e => e.ClassroomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Section)
                    .WithMany()
                    .HasForeignKey(e => e.SectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------- Teacher domain configuration ----------------

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(50);

                // optional assigned class
                entity.HasOne(t => t.ClassAssigned)
                    .WithMany()
                    .HasForeignKey(t => t.ClassAssignedId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TeacherAttendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).HasColumnType("date");
                entity.HasOne(a => a.Teacher)
                      .WithMany(t => t.Attendances)
                      .HasForeignKey(a => a.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TeacherSalary>(entity =>
            {
                entity.HasKey(e => e.Id);
                // SalaryMonth stored as date (we typically store first-day-of-month)
                entity.Property(e => e.SalaryMonth).HasColumnType("date");
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaidAt).HasColumnType("datetime(6)");
                entity.HasOne(s => s.Teacher)
                      .WithMany(t => t.Salaries)
                      .HasForeignKey(s => s.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            

            // --------------- TeacherSubject mapping ---------------
            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SubjectName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(ts => ts.Teacher)
                      .WithMany(t => t.TeacherSubjects)
                      .HasForeignKey(ts => ts.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);


                entity.HasOne(ts => ts.Subject)
                      .WithMany()
                      .HasForeignKey(ts => ts.SubjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ts => ts.Classroom)
                      .WithMany()
                      .HasForeignKey(ts => ts.ClassroomId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ts => ts.Section)
      .WithMany()
      .HasForeignKey(ts => ts.SectionId)
      .OnDelete(DeleteBehavior.Cascade);

            });

            // ---------------- Seed Classrooms (1..10) and Sections (A..D each) ----------------

            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { Id = 1, Name = "Class 1" },
                new Classroom { Id = 2, Name = "Class 2" },
                new Classroom { Id = 3, Name = "Class 3" },
                new Classroom { Id = 4, Name = "Class 4" },
                new Classroom { Id = 5, Name = "Class 5" },
                new Classroom { Id = 6, Name = "Class 6" },
                new Classroom { Id = 7, Name = "Class 7" },
                new Classroom { Id = 8, Name = "Class 8" },
                new Classroom { Id = 9, Name = "Class 9" },
                new Classroom { Id = 10, Name = "Class 10" }
            );

            // Seed sections A-D for each classroom (ids 1..40)
            var sectionId = 1;
            var sections = new List<Section>();
            for (int cls = 1; cls <= 10; cls++)
            {
                foreach (var name in new[] { "A", "B", "C", "D" })
                {
                    sections.Add(new Section
                    {
                        Id = sectionId++,
                        ClassroomId = cls,
                        Name = name
                    });
                }
            }
            modelBuilder.Entity<Section>().HasData(sections.ToArray());
        }
    }
}
