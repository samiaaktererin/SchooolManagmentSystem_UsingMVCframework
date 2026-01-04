using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;
using System.Linq;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Microsoft.AspNetCore.Authorization;

namespace MyMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AcademicController : Controller
    {
        private readonly AppDbContext _db;
        public AcademicController(AppDbContext db) => _db = db;

        // ================= DASHBOARD =================
        public IActionResult Index()
        {
            return View();
        }

        // ================= SUBJECT LIST =================
        public async Task<IActionResult> Subjects()
        {
            var list = await _db.Subjects
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(list);
        }

        // ================= CREATE SUBJECT =================
        public IActionResult CreateSubject()
        {
            return View(new Subject());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubject(Subject model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.Subjects.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Subjects));
        }

        // ================= ASSIGN SUBJECT =================
        public async Task<IActionResult> Assign(int? classId)
        {
            ViewBag.Teachers = await _db.Teachers
                .OrderBy(t => t.Name)
                .ToListAsync();

            ViewBag.Subjects = await _db.Subjects
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Classes = await _db.Classrooms
                .OrderBy(c => c.Id)
                .ToListAsync();

            // ✅ Class অনুযায়ী Section
            ViewBag.Sections = classId == null
                ? new List<MyMvcApp.Models.Section>()
                : await _db.Sections
                    .Where(s => s.ClassroomId == classId)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

            ViewBag.SelectedClassId = classId;

            return View();
        }


        // ================= ASSIGN POST =============


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(
    int teacherId,
    int subjectId,
    int classId,
    int sectionId)
        {
            bool exists = await _db.TeacherSubjects.AnyAsync(x =>
                x.TeacherId == teacherId &&
                x.SubjectId == subjectId &&
                x.ClassroomId == classId &&
                x.SectionId == sectionId);

            if (!exists)
            {
                _db.TeacherSubjects.Add(new TeacherSubject
                {
                    TeacherId = teacherId,
                    SubjectId = subjectId,
                    ClassroomId = classId,
                    SectionId = sectionId
                });

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AssignedList));
        }

        // ================= ASSIGNED LIST =================
        public async Task<IActionResult> AssignedList()
        {
            var list = await _db.TeacherSubjects
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .Include(x => x.Classroom)
                .Include(x => x.Section)
                .OrderBy(x => x.Teacher!.Name)
                .ToListAsync();

            return View(list);
        }


        // ================= ASSIGNMENT LIST =================
        public async Task<IActionResult> Assignments()
        {
            var list = await _db.TeacherSubjects
                .Include(ts => ts.Teacher)
                .Include(ts => ts.Subject)
                .Include(ts => ts.Classroom)
                .OrderBy(ts => ts.ClassroomId)
                .ToListAsync();

            return View(list);
        }
        // ================= EDIT ASSIGN (GET) =================
        public async Task<IActionResult> EditAssign(int id)
        {
            var assign = await _db.TeacherSubjects
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .Include(x => x.Classroom)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (assign == null) return NotFound();

            ViewBag.Teachers = await _db.Teachers.OrderBy(t => t.Name).ToListAsync();
            ViewBag.Subjects = await _db.Subjects.OrderBy(s => s.Name).ToListAsync();
            ViewBag.Classes = await _db.Classrooms.OrderBy(c => c.Id).ToListAsync();

            ViewBag.Sections = await _db.Sections
                .Where(s => s.ClassroomId == assign.ClassroomId)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(assign);
        }


        // ================= EDIT ASSIGN (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssign(
    int id,
    int teacherId,
    int subjectId,
    int classId,
    int sectionId)
        {
            var assign = await _db.TeacherSubjects.FindAsync(id);
            if (assign == null) return NotFound();
            bool validSection = await _db.Sections.AnyAsync(s =>
    s.Id == sectionId && s.ClassroomId == classId);

            if (!validSection)
            {
                ModelState.AddModelError("", "Invalid section for selected class");
                return RedirectToAction(nameof(EditAssign), new { id });
            }


            assign.TeacherId = teacherId;
            assign.SubjectId = subjectId;
            assign.ClassroomId = classId;
            assign.SectionId = sectionId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(AssignedList));
        }

        // ================= AJAX: Get Sections by Class =================
        [HttpGet]
        public async Task<IActionResult> GetSections(int classId)
        {
            var sections = await _db.Sections
                .Where(s => s.ClassroomId == classId)
                .OrderBy(s => s.Name)
                .Select(s => new { id = s.Id, name = s.Name })
                .ToListAsync();

            return Json(sections);
        }


        //===========DElETE==============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssign(int id)
        {
            var item = await _db.TeacherSubjects.FindAsync(id);
            if (item != null)
            {
                _db.TeacherSubjects.Remove(item);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AssignedList));
        }





    }
}
