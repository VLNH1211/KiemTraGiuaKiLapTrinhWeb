using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin user
            var adminEmail = "admin@example.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Lập trình" },
                    new Category { Name = "Cơ sở dữ liệu" },
                    new Category { Name = "Mạng máy tính" },
                    new Category { Name = "Trí tuệ nhân tạo" },
                    new Category { Name = "Toán học" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Courses
            if (!await context.Courses.AnyAsync())
            {
                var courses = new List<Course>
                {
                    new Course
                    {
                        Name = "Lập trình C#",
                        Image = "https://images.unsplash.com/photo-1571171637578-41bc2dd41cd2?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "TS. Nguyễn Văn A",
                        CategoryId = 1
                    },
                    new Course
                    {
                        Name = "Lập trình Java",
                        Image = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "ThS. Trần Thị B",
                        CategoryId = 1
                    },
                    new Course
                    {
                        Name = "Lập trình Web",
                        Image = "https://images.unsplash.com/photo-1547082299-de196ea013d6?auto=format&fit=crop&w=400&q=80",
                        Credits = 4,
                        Lecturer = "TS. Lê Văn C",
                        CategoryId = 1
                    },
                    new Course
                    {
                        Name = "Cơ sở dữ liệu",
                        Image = "https://images.unsplash.com/photo-1544383835-bda2bc66a55d?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "PGS. Phạm Thị D",
                        CategoryId = 2
                    },
                    new Course
                    {
                        Name = "SQL Server nâng cao",
                        Image = "https://images.unsplash.com/photo-1607799279861-4dd421887fb3?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "TS. Hoàng Văn E",
                        CategoryId = 2
                    },
                    new Course
                    {
                        Name = "Mạng máy tính",
                        Image = "https://images.unsplash.com/photo-1544197150-b99a580bb7a8?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "ThS. Vũ Thị F",
                        CategoryId = 3
                    },
                    new Course
                    {
                        Name = "An toàn mạng",
                        Image = "https://images.unsplash.com/photo-1563986768609-322da13575f3?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "TS. Đỗ Văn G",
                        CategoryId = 3
                    },
                    new Course
                    {
                        Name = "Trí tuệ nhân tạo",
                        Image = "https://images.unsplash.com/photo-1677442136019-21780efad99a?auto=format&fit=crop&w=400&q=80",
                        Credits = 4,
                        Lecturer = "PGS. Ngô Thị H",
                        CategoryId = 4
                    },
                    new Course
                    {
                        Name = "Machine Learning",
                        Image = "https://images.unsplash.com/photo-1527474305487-b87b222841cc?auto=format&fit=crop&w=400&q=80",
                        Credits = 4,
                        Lecturer = "TS. Bùi Văn I",
                        CategoryId = 4
                    },
                    new Course
                    {
                        Name = "Giải tích",
                        Image = "https://images.unsplash.com/photo-1635070041078-e363dbe005cb?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "PGS. Lý Thị K",
                        CategoryId = 5
                    },
                    new Course
                    {
                        Name = "Đại số tuyến tính",
                        Image = "https://images.unsplash.com/photo-1509228468518-180dd4864904?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "TS. Trương Văn L",
                        CategoryId = 5
                    },
                    new Course
                    {
                        Name = "Xác suất thống kê",
                        Image = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=400&q=80",
                        Credits = 3,
                        Lecturer = "ThS. Mai Thị M",
                        CategoryId = 5
                    }
                };
                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }
            else
            {
                // Auto-upgrade existing courses to use the new premium Unsplash images
                var existingCourses = await context.Courses.ToListAsync();
                bool hasUpdates = false;
                foreach (var course in existingCourses)
                {
                    if (course.Image != null && course.Image.Contains(".svg"))
                    {
                        switch (course.Name)
                        {
                            case "Lập trình C#":
                                course.Image = "https://images.unsplash.com/photo-1571171637578-41bc2dd41cd2?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Lập trình Java":
                                course.Image = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Lập trình Web":
                                course.Image = "https://images.unsplash.com/photo-1547082299-de196ea013d6?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Cơ sở dữ liệu":
                                course.Image = "https://images.unsplash.com/photo-1544383835-bda2bc66a55d?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "SQL Server nâng cao":
                                course.Image = "https://images.unsplash.com/photo-1607799279861-4dd421887fb3?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Mạng máy tính":
                                course.Image = "https://images.unsplash.com/photo-1544197150-b99a580bb7a8?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "An toàn mạng":
                                course.Image = "https://images.unsplash.com/photo-1563986768609-322da13575f3?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Trí tuệ nhân tạo":
                                course.Image = "https://images.unsplash.com/photo-1677442136019-21780efad99a?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Machine Learning":
                                course.Image = "https://images.unsplash.com/photo-1527474305487-b87b222841cc?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Giải tích":
                                course.Image = "https://images.unsplash.com/photo-1635070041078-e363dbe005cb?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Đại số tuyến tính":
                                course.Image = "https://images.unsplash.com/photo-1509228468518-180dd4864904?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                            case "Xác suất thống kê":
                                course.Image = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=400&q=80";
                                hasUpdates = true;
                                break;
                        }
                    }
                }
                if (hasUpdates)
                {
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
