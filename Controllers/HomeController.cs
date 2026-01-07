using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        // Simple home page
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return View("AdminHome");

                if (User.IsInRole("Teacher"))
                    return RedirectToAction("MyProfile", "Teachers");
            }

            return View();
        }



        [Authorize(Roles = "Admin")]
        public IActionResult AdminHome()
        {
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult TeacherHome()
        {
            return View();
        }



        // =======================
        // DASHBOARD (READ-ONLY)
        // =======================
        public async Task<IActionResult> Dashboard()
        {
            var totalStudents = await _db.Students.CountAsync();
            var totalTeachers = await _db.Teachers.CountAsync();
            var activeTeachers = await _db.Teachers.CountAsync(t => t.IsActive);

            var today = DateTime.UtcNow.Date;
            var todayAttendanceCount = await _db.TeacherAttendances
                .CountAsync(a => a.Date == today && a.Present);

            var classList = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();

            var classTeacherMap = classList.Select(c =>
            {
                var teacher = _db.Teachers.FirstOrDefault(t => t.ClassAssignedId == c.Id);
                return new
                {
                    ClassName = c.Name,
                    TeacherName = teacher?.Name ?? "Not Assigned"
                };
            }).ToList();

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalTeachers = totalTeachers;
            ViewBag.ActiveTeachers = activeTeachers;
            ViewBag.TodayAttendance = todayAttendanceCount;
            ViewBag.ClassTeacherMap = classTeacherMap;

            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }



    }
}
