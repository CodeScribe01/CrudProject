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
    public class CourseManagementController: Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseManagementController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View(await _context.CourseManagement.OrderByDescending(c => c.CourseId).ToListAsync());
        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var course = await _context.CourseManagement.FindAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
            
                return RedirectToAction("Index", "Students");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CourseManagement model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);
            var course = await _context.CourseManagement.FirstOrDefaultAsync(c => c.CourseName == model.CourseName && c.Semester == model.Semester);
            if (course != null)
            {
                ModelState.AddModelError("", "Course already Exists.");
                return View(model);
            }


            _context.CourseManagement.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            if (id == null)
                return NotFound();
            var course = await _context.CourseManagement.FindAsync(id);
            if (course == null)
                return NotFound();
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id , CourseManagement model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (model == null)
                return NotFound();
            if (id != model.CourseId)
                return NotFound();

            if(!ModelState.IsValid)
                return View(model);

            var course = await _context.CourseManagement.FirstOrDefaultAsync(c=> c.CourseId==id);

            if(course == null)
                return NotFound();

            course.CourseName = model.CourseName;
            course.Semester = model.Semester;
            course.Fees = model.Fees;   

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var course = await _context.CourseManagement.FindAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var course = _context.CourseManagement.FirstOrDefault(c=> c.CourseId ==id);

            if(course == null)
                return NotFound(); 
            _context.CourseManagement.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
