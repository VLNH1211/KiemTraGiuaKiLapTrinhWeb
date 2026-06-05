using KiemTraGiuaKy.Data;
using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Controllers
{
    // Câu 2: CRUD Admin + Câu 4: Authorization + Câu 10: Dashboard
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // Câu 10: Dashboard thống kê
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalCourses = await _context.Courses.CountAsync();
            
            var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
            if (studentRole != null)
            {
                ViewBag.TotalStudents = await _context.UserRoles
                    .CountAsync(ur => ur.RoleId == studentRole.Id);
            }
            else
            {
                ViewBag.TotalStudents = 0;
            }

            ViewBag.TotalEnrollments = await _context.Enrollments.CountAsync();

            // Data for charts - enrollments per course
            var courseEnrollments = await _context.Courses
                .Select(c => new { c.Name, Count = c.Enrollments.Count })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            ViewBag.CourseNames = courseEnrollments.Select(c => c.Name).ToList();
            ViewBag.CourseCounts = courseEnrollments.Select(c => c.Count).ToList();

            // Enrollments per category
            var categoryEnrollments = await _context.Categories
                .Select(cat => new { cat.Name, Count = cat.Courses.SelectMany(c => c.Enrollments).Count() })
                .ToListAsync();

            ViewBag.CategoryNames = categoryEnrollments.Select(c => c.Name).ToList();
            ViewBag.CategoryCounts = categoryEnrollments.Select(c => c.Count).ToList();

            return View();
        }

        // Câu 2: Danh sách courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .OrderBy(c => c.Id)
                .ToListAsync();
            return View(courses);
        }

        // Câu 2: Create - GET
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // Câu 2: Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(_env.WebRootPath, "images", "courses", fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    course.Image = "/images/courses/" + fileName;
                }

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo học phần thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // Câu 2: Edit - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // Câu 2: Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course, IFormFile? imageFile)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(_env.WebRootPath, "images", "courses", fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        course.Image = "/images/courses/" + fileName;
                    }

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật học phần thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Courses.AnyAsync(c => c.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // Câu 2: Delete - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            return View(course);
        }

        // Câu 2: Delete - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa học phần thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
