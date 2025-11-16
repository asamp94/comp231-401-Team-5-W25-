using System.Linq;
using System.Threading.Tasks;
using Flow_App.Data;
using Flow_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flow_App.Controllers

    /*  TasksController handles all task-related webpage actions. It connects ot the database using FlowContext and lets users view, create, edit, and delete tasks. 
     *  Each public method represents a page or action in the Tasks section.
     * 
     */
{
    public class TasksController : Controller
    {
        private readonly FlowContext _context;

        public TasksController(FlowContext context)
        {
            _context = context;
        }

        //Retrieves all tasks from the database
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();

            return View(tasks);
        }

        //Shows empty form for user to enter a new task
        public IActionResult Create()
        {
            return View();
        }

        //Takes the submitted tasks form data and saves it to the database, then returning to task list
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

            return View(taskItem);
        }

       //Finds task by ID and displays it
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        //updates edited task in the database
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

            return View(taskItem);
        }

        // Removes the selected task from the database by ID.
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
