using System.Linq;
using System.Threading.Tasks;
using Flow_App.Data;
using Flow_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flow_App.Controllers
{   //
    /*  CoursesController handles all course-related webpage actions.
     *  Allows users to view, create, edit, and delete courses.
     */
    public class CoursesController : Controller
    {
        private readonly FlowContext _context;
        
        public CoursesController(FlowContext context) 
        {
            _context = context;
        }
    
    //Display listo f all courses with tasks
    public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Tasks)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(courses);
        }

    //Show empty form to creat a new course
    public IActionResult Create()
        {
            return View(); 
        }

    //Save new course to the database
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
       {
         if (ModelState.IsValid) 
          {
             _context.Add(course);
             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(Index));     
         }
            return View(course);
        }

    // Display edit form for existing course
    public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        //Update edit courses in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        //Delete course from the database
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //View all tasks for a specific course
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
    }

}

