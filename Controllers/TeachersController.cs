using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;
using MyMvcApp.Models.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;




namespace MyMvcApp.Controllers
{
    
    public class TeachersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private static DateTime BdToday()
        {
            var bdTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time")
            );
            return bdTime.Date;
        }

        private async Task GenerateMissingAttendance(int teacherId)
        {
            var startDate = new DateTime(2025, 12, 1); // Dec 1
            var today = GetBangladeshToday();

            var existingDates = await _db.TeacherAttendances
                .Where(a => a.TeacherId == teacherId)
                .Select(a => a.Date)
                .ToListAsync();

            var toInsert = new List<TeacherAttendance>();

            for (var d = startDate; d <= today; d = d.AddDays(1))
            {
                if (!existingDates.Contains(d))
                {
                    toInsert.Add(new TeacherAttendance
                    {
                        TeacherId = teacherId,
                        Date = d,
                        Present = false,
                        Note = "Auto Absent"
                    });
                }
            }

            if (toInsert.Any())
            {
                _db.TeacherAttendances.AddRange(toInsert);
                await _db.SaveChangesAsync();
            }
        }

        private DateTime GetBangladeshToday()
        {
            // Windows compatible
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;
        }



        public TeachersController(
            AppDbContext db,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ================= LIST =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? search)
        {
            var q = _db.Teachers
                .Include(t => t.ClassAssigned)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(t =>
                    (t.Name ?? "").Contains(search) ||
                    (t.Email ?? "").Contains(search) ||
                    (t.Phone ?? "").Contains(search));
            }

            return View(await q.OrderBy(t => t.Name).ToListAsync());
        }

        // ================= DETAILS =================
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _db.Teachers
    .Include(t => t.Attendances)
    .Include(t => t.Salaries)
    .Include(t => t.TeacherSubjects)
        .ThenInclude(ts => ts.Subject)
    .Include(t => t.TeacherSubjects)
        .ThenInclude(ts => ts.Classroom)
    .Include(t => t.TeacherSubjects)
        .ThenInclude(ts => ts.Section)
    .FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound();
            var today = GetBangladeshToday();

            bool alreadyMarkedToday = await _db.TeacherAttendances.AnyAsync(a =>
                a.TeacherId == teacher.Id &&
                a.Date == today);

            ViewBag.AlreadyMarkedToday = alreadyMarkedToday;


           
            return View(teacher);
        }

        // ================= CREATE =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();
            return View(new Teacher { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Teacher model,
    string password,
    IFormFile? photo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Classes = await _db.Classrooms
                    .OrderBy(c => c.Id)
                    .ToListAsync();
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Password is required.");
                ViewBag.Classes = await _db.Classrooms
                    .OrderBy(c => c.Id)
                    .ToListAsync();
                return View(model);
            }

            // ---------- PHOTO ----------
            if (photo != null && photo.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/teachers",
                    fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream);

                model.PhotoPath = "/uploads/teachers/" + fileName;
            }

            // ---------- SAVE TEACHER ----------
            _db.Teachers.Add(model);
            await _db.SaveChangesAsync();

            // ---------- CREATE LOGIN ----------
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

                ViewBag.Classes = await _db.Classrooms
                    .OrderBy(c => c.Id)
                    .ToListAsync();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Teacher");

            return RedirectToAction(nameof(Index));
        }



        // ================= EDIT =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher model, IFormFile? photo)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();
                return View(model);
            }

            var t = await _db.Teachers.FindAsync(id);
            if (t == null) return NotFound();

            t.Name = model.Name;
            t.Email = model.Email;
            t.Phone = model.Phone;
            t.IsActive = model.IsActive;
            t.ClassAssignedId = model.ClassAssignedId;

            if (photo != null && photo.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/teachers",
                    fileName
                );

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream);

                t.PhotoPath = "/uploads/teachers/" + fileName;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = t.Id });
        }


        // ================= ATTENDANCE =================
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Attendance(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            ViewBag.Teacher = teacher;

            var today = GetBangladeshToday();
            var startDate = new DateTime(today.Year, 12, 1); // Dec 1

            var dbAttendance = await _db.TeacherAttendances
                .Where(a => a.TeacherId == id)
                .ToListAsync();

            var finalList = new List<TeacherAttendance>();

            for (var d = startDate; d <= today; d = d.AddDays(1))
            {
                var existing = dbAttendance.FirstOrDefault(a => a.Date == d);

                if (existing != null)
                {
                    finalList.Add(existing);
                }
                else
                {
                    // 🔴 AUTO ABSENT (no DB insert)
                    finalList.Add(new TeacherAttendance
                    {
                        TeacherId = id,
                        Date = d,
                        Present = false,
                        Note = "Auto Absent"
                    });
                }
            }

            return View(finalList.OrderByDescending(x => x.Date));
        }




        // ✅ ATTENDANCE SAVE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordAttendance(
    int teacherId,
    DateTime date,
    bool present,
    string? note)
        {
            var teacher = await _db.Teachers.FindAsync(teacherId);
            if (teacher == null) return NotFound();

            // 🔴 NEW: inactive teacher → block save
            if (!teacher.IsActive)
            {
                TempData["AttendanceError"] = "This teacher is inactive. Attendance cannot be recorded.";
                return RedirectToAction(nameof(Attendance), new { id = teacherId });
            }

            var existing = await _db.TeacherAttendances
                .FirstOrDefaultAsync(a =>
                    a.TeacherId == teacherId &&
                    a.Date == date.Date);

            if (existing != null)
            {
                existing.Present = present;
                existing.Note = note;
            }
            else
            {
                _db.TeacherAttendances.Add(new TeacherAttendance
                {
                    TeacherId = teacherId,
                    Date = date.Date,
                    Present = present,
                    Note = note
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Attendance), new { id = teacherId });
        }


        // ================= SALARY =================
        [Authorize(Roles = "Admin,Teacher")]

        public async Task<IActionResult> Salary(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            ViewBag.Teacher = teacher;

            var list = await _db.TeacherSalaries
                .Where(s => s.TeacherId == id)
                .OrderByDescending(s => s.PaidAt)
                .ToListAsync();

            return View(list);

            ;
        }

        // ✅ SALARY SAVE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> AddSalary(
            int teacherId,
            decimal amount,
            DateTime salaryMonth,
            string? paymentMethod,
            string? note)
        {
            _db.TeacherSalaries.Add(new TeacherSalary
            {
                TeacherId = teacherId,
                Amount = amount,
                SalaryMonth = salaryMonth,
                PaymentMethod = paymentMethod,
                Note = note,
                PaidAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Salary), new { id = teacherId });
        }

        // ================= DELETE =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.Teachers.FindAsync(id);
            if (t == null) return NotFound();
            return View(t);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t = await _db.Teachers.FindAsync(id);
            if (t == null) return NotFound();

            _db.Teachers.Remove(t);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> MyProfile()
        {
            var email = User.Identity!.Name;

            var teacher = await _db.Teachers
                .Include(t => t.Attendances)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Classroom)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Section)
                .FirstOrDefaultAsync(t => t.Email == email);

            if (teacher == null)
                return Unauthorized();

            // ✅ FIXED: Local date
            // ================= AUTO ABSENT (ONLY AFTER DAY ENDS) =================
            var bdTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time")
            );

            // আজকের তারিখ
            var today = bdTime.Date;

            // আগের দিন
            var yesterday = today.AddDays(-1);

            // আগের দিনের attendance আছে কিনা
            bool hasYesterdayAttendance = await _db.TeacherAttendances.AnyAsync(a =>
                a.TeacherId == teacher.Id &&
                a.Date == yesterday
            );

            // যদি আগের দিনের attendance না থাকে → Auto Absent
            if (!hasYesterdayAttendance)
            {
                _db.TeacherAttendances.Add(new TeacherAttendance
                {
                    TeacherId = teacher.Id,
                    Date = yesterday,
                    Present = false,
                    Note = "Auto Absent"
                });

                await _db.SaveChangesAsync();
            }


            bool alreadyMarkedToday = await _db.TeacherAttendances.AnyAsync(a =>
    a.TeacherId == teacher.Id &&
    a.Date == today &&
    a.Present == true
);


            ViewBag.AlreadyMarkedToday = alreadyMarkedToday;


           

            ViewBag.AlreadyMarkedToday = alreadyMarkedToday;


            ViewBag.AlreadyMarkedToday = alreadyMarkedToday;

            return View("Details", teacher);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPresent()
        {
            var email = User.Identity!.Name;
            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Email == email);
            if (teacher == null) return Unauthorized();

            var today = DateTime.Today; // IMPORTANT

            var attendance = await _db.TeacherAttendances
                .FirstOrDefaultAsync(a =>
                    a.TeacherId == teacher.Id &&
                    a.Date == today);

            if (attendance == null)
            {
                // ❗ Only create if NOT exists
                _db.TeacherAttendances.Add(new TeacherAttendance
                {
                    TeacherId = teacher.Id,
                    Date = today,
                    Present = true
                });
            }
            else
            {
                // ❗ Update existing (auto absent → present)
                attendance.Present = true;
                attendance.Note = null;
            }

            await _db.SaveChangesAsync();

            TempData["AttendanceSuccess"] = "Your attendance has been recorded successfully.";
            return RedirectToAction("MyProfile");
        }




        // ================= ADMIN: VIEW SINGLE TEACHER ATTENDANCE =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AttendanceByTeacher(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            ViewBag.Teacher = teacher;

            var list = await _db.TeacherAttendances
                .Where(a => a.TeacherId == id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View("AttendanceList", list);
        }

        // ================= ADMIN: VIEW ALL TEACHER ATTENDANCE =================


        // ================= ADMIN: EDIT / ADD ATTENDANCE =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAttendance(int teacherId, DateTime date)
        {
            var attendance = await _db.TeacherAttendances
                .FirstOrDefaultAsync(a =>
                    a.TeacherId == teacherId &&
                    a.Date == date.Date);

            // যদি attendance না থাকে → new বানাও (missed date)
            if (attendance == null)
            {
                attendance = new TeacherAttendance
                {
                    TeacherId = teacherId,
                    Date = date.Date,
                    Present = false
                };
            }

            ViewBag.Teacher = await _db.Teachers.FindAsync(teacherId);
            return View(attendance);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttendance(TeacherAttendance model)
        {
            var existing = await _db.TeacherAttendances
                .FirstOrDefaultAsync(a =>
                    a.TeacherId == model.TeacherId &&
                    a.Date == model.Date);

            if (existing == null)
            {
                // 🔹 Admin add করছে missed attendance
                _db.TeacherAttendances.Add(model);
            }
            else
            {
                // 🔹 Admin edit করছে existing attendance
                existing.Present = model.Present;
                existing.Note = model.Note;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("AttendanceList", new { id = model.TeacherId });

        }
        // ================= ADMIN: ADD MISSED ATTENDANCE =================


        // ================= ADMIN: ADD MISSED ATTENDANCE (FIXED TEACHER) =================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMissedAttendance(int teacherId)
        {
            var teacher = await _db.Teachers.FindAsync(teacherId);
            if (teacher == null) return NotFound();

            ViewBag.Teacher = teacher;

            var model = new TeacherAttendance
            {
                TeacherId = teacherId,
                Date = DateTime.UtcNow.Date,
                Present = false
            };

            return View(model);
        }
        [HttpPost]
[Authorize(Roles = "Admin")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddMissedAttendance(TeacherAttendance model)
{
    var exists = await _db.TeacherAttendances.AnyAsync(a =>
        a.TeacherId == model.TeacherId &&
        a.Date == model.Date);

    if (!exists)
    {
        _db.TeacherAttendances.Add(model);
        await _db.SaveChangesAsync();
    }

    return RedirectToAction("AttendanceList", new { id = model.TeacherId });
}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AttendanceList(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            // 🔥 AUTO ABSENT GENERATE
            await GenerateMissingAttendance(id);

            ViewBag.Teacher = teacher;

            var list = await _db.TeacherAttendances
                .Where(a => a.TeacherId == id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(list);
        }
        // ================= CHANGE PASSWORD (GET) =================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            ViewBag.TeacherId = id;
            ViewBag.TeacherName = teacher.Name;

            return View();
        }


        // ================= CHANGE PASSWORD (POST) =================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["Error"] = "Password cannot be empty";
                return RedirectToAction(nameof(ChangePassword), new { id });
            }

            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            var user = await _userManager.FindByEmailAsync(teacher.Email);
            if (user == null)
            {
                TempData["Error"] = "User not found in AspNetUsers";
                return RedirectToAction(nameof(ChangePassword), new { id });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(ChangePassword), new { id });
            }

            TempData["Success"] = "✅ Password updated successfully";
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FixOldTeachers()
        {
            var teachers = await _db.Teachers.ToListAsync();

            foreach (var t in teachers)
            {
                if (string.IsNullOrWhiteSpace(t.Email))
                    continue;

                var user = await _userManager.FindByEmailAsync(t.Email);

                if (user == null)
                {
                    var newUser = new IdentityUser
                    {
                        UserName = t.Email,
                        Email = t.Email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(newUser, "Temp@123");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, "Teacher");
                    }
                }
            }

            return Content("✅ Old teachers fixed. Default password = Temp@123");
        }
        public async Task AutoMarkAbsentForYesterday()
        {
            var yesterday = DateTime.Today.AddDays(-1);

            var teachers = await _db.Teachers
                .Where(t => t.IsActive)
                .Select(t => t.Id)
                .ToListAsync();

            foreach (var teacherId in teachers)
            {
                bool exists = await _db.TeacherAttendances.AnyAsync(a =>
                    a.TeacherId == teacherId &&
                    a.Date == yesterday);

                if (!exists)
                {
                    _db.TeacherAttendances.Add(new TeacherAttendance
                    {
                        TeacherId = teacherId,
                        Date = yesterday,
                        Present = false,
                        Note = "Auto Absent"
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.UtcNow.Date;

            var teachers = await _db.Teachers.ToListAsync();
            var attendanceToday = await _db.TeacherAttendances
                .Where(a => a.Date == today)
                .ToListAsync();

            // ===== summary counts =====
            ViewBag.TotalTeachers = teachers.Count;
            ViewBag.ActiveTeachers = teachers.Count(t => t.IsActive);
            ViewBag.InactiveTeachers = teachers.Count(t => !t.IsActive);

            ViewBag.PresentToday = attendanceToday.Count(a => a.Present);
            ViewBag.AbsentToday = teachers.Count - ViewBag.PresentToday;

            // ===== list data =====
            var list = teachers.Select(t =>
            {
                var att = attendanceToday.FirstOrDefault(a => a.TeacherId == t.Id);

                return new TeacherDashboardRowVM
                {
                    TeacherName = t.Name,
                    IsActive = t.IsActive,
                    IsPresentToday = att != null && att.Present
                };
            }).ToList();

            return View(list);
        }









    }
}
