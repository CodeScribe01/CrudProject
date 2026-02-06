using CrudProject.Data;
using CrudProject.Models;
using CrudProject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace CrudProject.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeachersController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsLoggenIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");

            var teachers = await _context.Teachers.ToListAsync();
            return View(teachers);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound();
            return View(teacher);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsLoggenIn())
                return RedirectToAction("Index", "Teachers");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teachers model)
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            if (!Validation.IsValidEmail(model.Email))
                ModelState.AddModelError("Email", "Invalid Email");

            if (!Validation.IsValidPhone(model.Phone))
                ModelState.AddModelError("Phone", "Invalid Phone Number");

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == model.Email || t.Phone == model.Phone);
            if (teacher != null)
            {
                ModelState.AddModelError("IsActive", "Email Or Phone already exists.");
                return View(model);
            }

            _context.Teachers.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");
            if (id == null)
                return NotFound();
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound();
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teachers model)
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");

            if (model == null)
                return NotFound();
            if (id != model.TeacherId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            if (!Validation.IsValidEmail(model.Email))
                ModelState.AddModelError("Email", "Invalid Email");

            if (!Validation.IsValidPhone(model.Phone))
                ModelState.AddModelError("Phone", "Invalid Phone Number");

            var teacher = await _context.Teachers.FirstOrDefaultAsync(c => c.Email == model.Email || c.Phone == model.Phone);
            if (teacher == null)
                return NotFound();
            if (teacher != null)
            {
                ModelState.AddModelError("IsActive", "Email Or Phone already exists.");
                return View(model);
            }
            teacher.Name = model.Name;
            teacher.Phone = model.Phone;
            teacher.Salary = model.Salary;
            teacher.Email = model.Email;
            teacher.JoiningDate = model.JoiningDate;
            teacher.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var teachers = await _context.Teachers.FindAsync(id);
            if(teachers == null)
                return NotFound();

            return View(teachers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggenIn())
                return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var teachers = await _context.Teachers.FindAsync(id);
            if (teachers == null)
                return NotFound();

            _context.Teachers.Remove(teachers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Mappinggg

        public IActionResult MapCourses()
        {
            ViewData["Teachers"] = _context.Teachers.Where(t => t.IsActive == true).ToList();

            ViewData["Course"] = _context.CourseManagement.ToList();

            var mappedData = (from m in _context.TeacherCourseMapping
                              join t in _context.Teachers
                                    on m.TeacherId equals t.TeacherId
                              join c in _context.CourseManagement
                                on m.CourseId equals c.CourseId
                              select new
                              {
                                  TeacherName = t.Name,
                                  CourseName = c.CourseName
                              }).ToList();
            ViewData["MappedList"] = mappedData;                

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MapCourses(TeacherCourseMapping model)
        {
            if (model.TeacherId == 0)
                ModelState.AddModelError("TeacherId", "Select a teacher");
            if (model.CourseId == 0)
                ModelState.AddModelError("CourseId", "Select a course");
           
            bool exists = await _context.TeacherCourseMapping
                .AnyAsync(x => x.TeacherId == model.TeacherId
                            && x.CourseId == model.CourseId);

            if (exists)
                ModelState.AddModelError("TeacherId", "Teacher is already mapped with selected course.");

            if (!ModelState.IsValid)
            {
                ViewData["Teachers"] = _context.Teachers
                                       .Where(t => t.IsActive == true)
                                       .ToList();

                ViewData["Course"] = _context.CourseManagement.ToList();

                ViewData["MappedList"] = (from m in _context.TeacherCourseMapping
                                          join t in _context.Teachers
                                              on m.TeacherId equals t.TeacherId
                                          join c in _context.CourseManagement
                                              on m.CourseId equals c.CourseId
                                          select new
                                          {
                                              TeacherName = t.Name,
                                              CourseName = c.CourseName,
                                              Status = m.IsActive
                                          }).ToList();

                return View(model);
            }

            var mapping = new TeacherCourseMapping
            {
                TeacherId = model.TeacherId,
                CourseId = model.CourseId,
                IsActive = true
            };

            _context.TeacherCourseMapping.Add(mapping);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MapCourses));
        }


        public class Validation
        {
            public static bool IsValidEmail(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }
                for (int i = 0; i < email.Length; i++)
                {
                    if (email[i] == ' ')
                        return false;
                }

                int atCnt = 0;
                int atIndex = -1;

                for (int i = 0; i < email.Length; i++)
                {
                    if (email[i] == '@')
                    {
                        atCnt++;
                        atIndex = i;
                    }
                }

                if (atCnt != 1)
                    return false;

                if (atIndex == 0 || atIndex == email.Length - 1)
                    return false;

                bool dotFound = false;

                for (int i = atIndex + 1; i < email.Length; i++)
                {
                    if (email[i] == '.')
                    {
                        dotFound = true;
                        break;
                    }
                }

                if (!dotFound)
                    return false;

                if (email[atIndex + 1] == '.' || email[email.Length - 1] == '.')
                    return false;

                if (!char.IsLetterOrDigit(email[0]))
                    return false;

                for (int i = 0; i < email.Length; i++)
                {
                    char c = email[i];

                    if (!(char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '_' || c == '-'))
                    {
                        return false;
                    }
                }

                int dotCount = 0;

                for (int i = 0; i < email.Length; i++)
                {
                    if (email[i] == '.')
                    {
                        dotCount++;
                    }
                }

                if (dotCount >= 3)
                    return false;


                return true;


            }



            public static bool IsValidPhone(string phone)
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return false;

                if (phone.Length != 10)
                    return false;

                for (int i = 0; i < phone.Length; i++)
                {
                    if (!char.IsDigit(phone[i]))
                        return false;
                }

                return true;
            }
        }

    }
}
