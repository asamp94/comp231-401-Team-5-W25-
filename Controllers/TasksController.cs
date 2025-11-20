using System.Linq;
using System.Threading.Tasks;
using Flow_App.Data;
using Flow_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Flow_App.Controllers
{
    /*  TasksController handles all task-related webpage actions. It connects to the database using FlowContext 
     *  and lets users view, create, edit, and delete tasks with course filtering support.
     */
    public class TasksController : Controller
    {
        private readonly FlowContext _context;

        public TasksController(FlowContext context)
        {
            _context = context;
        }

        // Retrieves all tasks from the database with optional course filter
        public async Task<IActionResult> Index(int? courseId)
        {
            var tasksQuery = _context.Tasks
                .Include(t => t.Course)
                .AsQueryable();

            // Filter by course if courseId is provided
            if (courseId.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.CourseId == courseId.Value);
                var course = await _context.Courses.FindAsync(courseId.Value);
                ViewData["FilteredCourse"] = course?.Name;
            }

            var tasks = await tasksQuery
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();

            // Pass list of courses for filter dropdown
            ViewBag.Courses = await _context.Courses.ToListAsync();
            ViewBag.SelectedCourseId = courseId;

            return View(tasks);
        }

        // Shows empty form for user to enter a new task
        public async Task<IActionResult> Create()
        {
            // Load courses for dropdown
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            return View();
        }

        // Takes the submitted task form data and saves it to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload courses if validation fails
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            return View(taskItem);
        }

        // Finds task by ID and displays it
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Load courses for dropdown
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name", task.CourseId);
            return View(task);
        }

        // Updates edited task in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload courses if validation fails
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name", taskItem.CourseId);
            return View(taskItem);
        }

        // Removes the selected task from the database by ID
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}