using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KiemTraGiuaKy.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        public DateTime EnrollDate { get; set; } = DateTime.Now;

        public IdentityUser User { get; set; } = null!;

        public Course Course { get; set; } = null!;
    }
}
