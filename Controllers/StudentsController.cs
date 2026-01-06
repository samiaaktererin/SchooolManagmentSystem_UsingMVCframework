using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public StudentsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // INDEX (list all students; provides ViewBag.Classes for sidebar)
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _db.Students
                           .Include(s => s.Classroom)
                           .Include(s => s.Section)
                           .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    (s.Name ?? string.Empty).Contains(search) ||
                    (s.Email ?? string.Empty).Contains(search) ||
                    (s.Roll ?? string.Empty).Contains(search));
            }

            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var list = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Pagination = new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Search = search
            };

            // ---------- CLASSES + SECTIONS LOAD ----------
            var classesList = await _db.Classrooms
                .OrderBy(c => c.Id)
                .ToListAsync();

            var allSections = await _db.Sections
                .OrderBy(s => s.ClassroomId).ThenBy(s => s.Name)
                .ToListAsync();

            foreach (var cls in classesList)
            {
                cls.Sections = allSections.Where(s => s.ClassroomId == cls.Id).ToList();
            }

            // student counts per section for sidebar badges (optional)
            var sectionCounts = await _db.Students
                .Where(s => s.SectionId != null)
                .GroupBy(s => s.SectionId)
                .Select(g => new { SectionId = g.Key, Count = g.Count() })
                .ToListAsync();

            var sectionCountDict = sectionCounts
                .Where(x => x.SectionId.HasValue)
                .ToDictionary(x => x.SectionId!.Value, x => x.Count);

            ViewBag.SectionCounts = sectionCountDict;
            ViewBag.Classes = classesList;
            // ---------- END CLASSES + SECTIONS LOAD ----------

            return View(list);
        }

        // BYCLASS: filtered view for a specific class with section/roll/name filters + sorting
        public async Task<IActionResult> ByClass(int id, int? sectionId, string? roll, string? name, string sort = "roll_asc")
        {
            var classroom = await _db.Classrooms.FindAsync(id);
            if (classroom == null) return NotFound();

            var query = _db.Students
                           .Include(s => s.Section)
                           .Include(s => s.Classroom)
                           .Where(s => s.ClassroomId == id)
                           .AsQueryable();

            if (sectionId.HasValue)
                query = query.Where(s => s.SectionId == sectionId.Value);

            if (!string.IsNullOrWhiteSpace(roll))
                query = query.Where(s => (s.Roll ?? string.Empty).Contains(roll));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => (s.Name ?? string.Empty).Contains(name));

            // sorting numeric-aware on Roll (stored as string)
            List<Student> students;
            if (sort == "roll_desc")
            {
                students = query
                    .AsEnumerable()
                    .OrderByDescending(s => int.TryParse(s.Roll, out var v) ? v : int.MinValue)
                    .ThenBy(s => s.Name)
                    .ToList();
            }
            else
            {
                students = query
                    .AsEnumerable()
                    .OrderBy(s => int.TryParse(s.Roll, out var v) ? v : int.MaxValue)
                    .ThenBy(s => s.Name)
                    .ToList();
            }

            // Prepare view data (so view doesn't need Request)
            ViewBag.Classroom = classroom;
            ViewBag.SectionsForClass = await _db.Sections
                .Where(s => s.ClassroomId == id)
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewData["CurrentSort"] = sort;
            ViewData["RollSortParam"] = sort == "roll_asc" ? "roll_desc" : "roll_asc";
            ViewData["ClassId"] = id;

            // add filter values to ViewData so view can pre-fill form fields
            ViewData["SectionId"] = sectionId;
            ViewData["FilterRoll"] = roll ?? string.Empty;
            ViewData["FilterName"] = name ?? string.Empty;

            return View(students);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();
            ViewBag.Sections = await _db.Sections.OrderBy(s => s.Id).ToListAsync();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Student model,
    IFormFile? photo,
    int? classId,
    int? sectionId
)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();
                ViewBag.Sections = await _db.Sections.OrderBy(s => s.Id).ToListAsync();
                return View(model);
            }

            // photo upload
            if (photo != null && photo.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "images", "students");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(folder, fileName);
                using var stream = System.IO.File.Create(filePath);
                await photo.CopyToAsync(stream);

                model.PhotoPath = $"/images/students/{fileName}";
            }

            model.ClassroomId = classId;
            model.SectionId = sectionId;
            model.CreatedAt = DateTime.UtcNow;

            // ✅ ParentInfo auto bind হবে
            if (model.ParentInfo != null &&
                string.IsNullOrWhiteSpace(model.ParentInfo.FatherName))
            {
                model.ParentInfo = null;
            }
            ModelState.Remove("ParentInfo.Student");
            ModelState.Remove("ParentInfo.StudentId");


            _db.Students.Add(model);
            await _db.SaveChangesAsync();

            if (classId.HasValue && sectionId.HasValue)
            {
                _db.EnrollmentHistories.Add(new EnrollmentHistory
                {
                    StudentId = model.Id,
                    ClassroomId = classId.Value,
                    SectionId = sectionId.Value,
                    EnrolledAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var student = await _db.Students
                .Include(s => s.ParentInfo)
                .Include(s => s.Classroom)
                .Include(s => s.Section)
                .Include(s => s.EnrollmentHistories).ThenInclude(e => e.Classroom)
                .Include(s => s.EnrollmentHistories).ThenInclude(e => e.Section)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _db.Students.Include(s => s.ParentInfo).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();

            ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();

            if (student.ClassroomId.HasValue)
            {
                ViewBag.Sections = await _db.Sections
                    .Where(s => s.ClassroomId == student.ClassroomId.Value)
                    .OrderBy(s => s.Id)
                    .ToListAsync();
            }
            else
            {
                ViewBag.Sections = await _db.Sections.OrderBy(s => s.Id).ToListAsync();
            }

            return View(student);
        }

        // POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    Student model,
    IFormFile? photo,
    int? classId,
    int? sectionId,
    bool? removePhoto)
        {
            if (id != model.Id) return BadRequest();

            var student = await _db.Students
                .Include(s => s.ParentInfo)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            // 🔴 IMPORTANT FIX
            ModelState.Remove("ParentInfo.Student");
            ModelState.Remove("ParentInfo.StudentId");

            if (!ModelState.IsValid)
            {
                ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();
                ViewBag.Sections = await _db.Sections.OrderBy(s => s.Id).ToListAsync();
                return View(model);
            }

            // ================= BASIC FIELDS =================
            student.Name = model.Name;
            student.Email = model.Email;
            student.Roll = model.Roll;
            student.IsActive = model.IsActive;

            // ================= PHOTO =================
            if (removePhoto == true && !string.IsNullOrEmpty(student.PhotoPath))
            {
                var oldPath = Path.Combine(
                    _env.WebRootPath,
                    student.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                student.PhotoPath = null;
            }

            if (photo != null && photo.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "images", "students");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(folder, fileName);

                using var stream = System.IO.File.Create(filePath);
                await photo.CopyToAsync(stream);

                student.PhotoPath = $"/images/students/{fileName}";
            }

            // ================= PARENT INFO =================
            if (model.ParentInfo != null &&
                !string.IsNullOrWhiteSpace(model.ParentInfo.FatherName))
            {
                if (student.ParentInfo == null)
                {
                    student.ParentInfo = new ParentInfo
                    {
                        FatherName = model.ParentInfo.FatherName,
                        FatherPhone = model.ParentInfo.FatherPhone
                    };
                }
                else
                {
                    student.ParentInfo.FatherName = model.ParentInfo.FatherName;
                    student.ParentInfo.FatherPhone = model.ParentInfo.FatherPhone;
                }
            }

            // ================= CLASS / SECTION =================
            if (classId.HasValue && sectionId.HasValue)
            {
                if (student.ClassroomId != classId || student.SectionId != sectionId)
                {
                    student.ClassroomId = classId;
                    student.SectionId = sectionId;

                    _db.EnrollmentHistories.Add(new EnrollmentHistory
                    {
                        StudentId = student.Id,
                        ClassroomId = classId.Value,
                        SectionId = sectionId.Value,
                        EnrolledAt = DateTime.UtcNow
                    });
                }
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = student.Id });
        }


        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _db.Students.Include(s => s.ParentInfo).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();

            if (!string.IsNullOrEmpty(student.PhotoPath))
            {
                var oldPath = Path.Combine(_env.WebRootPath, student.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            _db.EnrollmentHistories.RemoveRange(_db.EnrollmentHistories.Where(e => e.StudentId == id));
            if (student.ParentInfo != null) _db.Parents.Remove(student.ParentInfo);

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Toggle active/inactive (returns back to caller page if safe)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, string? returnUrl)
        {
            var s = await _db.Students.FindAsync(id);
            if (s == null) return NotFound();

            s.IsActive = !s.IsActive;
            await _db.SaveChangesAsync();

            // redirect back to previous page if it's a safe local URL
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // fallback
            return RedirectToAction(nameof(Index));
        }

        // JSON endpoint: Get sections for a class
        [HttpGet]
        public async Task<JsonResult> GetSections(int classId)
        {
            var sections = await _db.Sections
                                    .Where(s => s.ClassroomId == classId)
                                    .OrderBy(s => s.Name)
                                    .Select(s => new { id = s.Id, name = s.Name })
                                    .ToListAsync();
            return Json(sections);
        }
    }
}
