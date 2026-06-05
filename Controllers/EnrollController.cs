using KiemTraGiuaKy.Data;
using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Controllers
{
    // Câu 4: Authorization - chỉ Student truy cập /enroll/**
    // Câu 6: Đăng ký học phần + Câu 7: My Courses
    [Authorize(Roles = "Student")]
    public class EnrollController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EnrollController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Câu 6: Đăng ký học phần
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId, string? returnUrl)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            // Check if already enrolled
            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (existing == null)
            {
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = courseId,
                    EnrollDate = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đăng ký học phần thành công!";
            }
            else
            {
                TempData["Info"] = "Bạn đã đăng ký học phần này rồi.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // Câu 6: Hủy đăng ký học phần
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(int courseId, string? returnUrl)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hủy đăng ký học phần thành công!";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // Câu 7: My Courses - danh sách học phần đã đăng ký
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);
            
            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .OrderByDescending(e => e.EnrollDate)
                .ToListAsync();

            return View(enrollments);
        }
    }
}
