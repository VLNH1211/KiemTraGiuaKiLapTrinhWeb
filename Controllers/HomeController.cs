using KiemTraGiuaKy.Data;
using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KiemTraGiuaKy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Câu 1: Trang Home hiển thị danh sách học phần + Câu 8: Tìm kiếm
        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            const int pageSize = 5;

            var query = _context.Courses
                .Include(c => c.Category)
                .AsQueryable();

            // Câu 8: Tìm kiếm theo tên học phần
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }

            query = query.OrderBy(c => c.Id);

            var paginatedCourses = await PaginatedList<Course>.CreateAsync(query, page, pageSize);

            // Get enrolled course IDs for the current user (for Enroll button)
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                var enrolledCourseIds = await _context.Enrollments
                    .Where(e => e.UserId == userId)
                    .Select(e => e.CourseId)
                    .ToListAsync();
                ViewBag.EnrolledCourseIds = enrolledCourseIds;
            }

            ViewBag.CurrentSearch = search;
            return View(paginatedCourses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
