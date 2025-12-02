using System.Linq;
using System.Threading.Tasks;
using Flow_App.Data;
using Flow_App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Flow_App.Controllers
{
    /* TasksController handles all task-related webpage actions. It connects to the database using FlowContext 
     * and lets users view, create, edit, and delete tasks with course filtering support.
     */
    public class TasksController : Controller
    {
        private readonly FlowContext _context;

        public TasksController(FlowContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? courseId)
        {
            var tasksQuery = _context.Tasks
                .Include(t => t.Course)
                .AsQueryable();

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

            ViewBag.Courses = await _context.Courses.ToListAsync();
            ViewBag.SelectedCourseId = courseId;

            // Get ALL tasks with reminders - use explicit property names for JavaScript compatibility
            var allTasksWithReminders = await _context.Tasks
                .Where(t => t.ReminderEnabled && t.DueDate.HasValue)
                .Select(t => new
                {
                    id = t.Id,
                    title = t.Title,
                    description = t.Description,
                    reminderEnabled = t.ReminderEnabled,
                    dueDate = t.DueDate.Value,
                    reminderMinutesBefore = t.ReminderMinutesBefore,
                    reminderTime = t.DueDate.Value.AddMinutes(-t.ReminderMinutesBefore)
                })
                .ToListAsync();

            // Debug: Log to console
            System.Diagnostics.Debug.WriteLine($"Found {allTasksWithReminders.Count} tasks with reminders");
            foreach (var task in allTasksWithReminders)
            {
                System.Diagnostics.Debug.WriteLine($"Task: {task.title}, Reminder Time: {task.reminderTime}");
            }

            ViewBag.TasksWithReminder = allTasksWithReminders;

            return View(tasks);
        }


        // Shows empty form for creating a new task
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            return View();
        }

        // Handles POST of new task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem taskItem)
        {
            if (taskItem.ReminderEnabled)
            {
                taskItem.ReminderMinutesBefore = taskItem.ReminderUnit switch
                {
                    "minutes" => taskItem.ReminderValue,
                    "hours" => taskItem.ReminderValue * 60,
                    "days" => taskItem.ReminderValue * 60 * 24,
                    _ => 60
                };
            }
            else
            {
                taskItem.ReminderMinutesBefore = 0;
            }

            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            return View(taskItem);
        }

        // Shows existing task for editing
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            // Convert minutes to value/unit for UI - prioritize smallest reasonable unit
            if (task.ReminderMinutesBefore > 0)
            {
                // Only use days if it's an exact number of days AND >= 1 day
                if (task.ReminderMinutesBefore >= 1440 && task.ReminderMinutesBefore % 1440 == 0)
                {
                    task.ReminderValue = task.ReminderMinutesBefore / 1440;
                    task.ReminderUnit = "days";
                }
                // Only use hours if it's an exact number of hours AND >= 1 hour
                else if (task.ReminderMinutesBefore >= 60 && task.ReminderMinutesBefore % 60 == 0)
                {
                    task.ReminderValue = task.ReminderMinutesBefore / 60;
                    task.ReminderUnit = "hours";
                }
                // Otherwise use minutes
                else
                {
                    task.ReminderValue = task.ReminderMinutesBefore;
                    task.ReminderUnit = "minutes";
                }
                task.ReminderEnabled = true;
            }
            else
            {
                // Default values when no reminder is set
                task.ReminderValue = 60;
                task.ReminderUnit = "minutes";
            }

            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name", task.CourseId);
            return View(task);
        }

        // Handles POST of edited task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id) return NotFound();

            if (taskItem.ReminderEnabled)
            {
                taskItem.ReminderMinutesBefore = taskItem.ReminderUnit switch
                {
                    "minutes" => taskItem.ReminderValue,
                    "hours" => taskItem.ReminderValue * 60,
                    "days" => taskItem.ReminderValue * 60 * 24,
                    _ => 60
                };
            }
            else
            {
                taskItem.ReminderMinutesBefore = 0;
            }

            if (ModelState.IsValid)
            {
                _context.Update(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name", taskItem.CourseId);
            return View(taskItem);
        }

        // Deletes a task by ID
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

        // Calendar view with tasks that have a due date
        public async Task<IActionResult> Calendar()
        {
            var tasks = await _context.Tasks
                .Include(t => t.Course)
                .Where(t => t.DueDate != null)
                .ToListAsync();

            return View(tasks);
        }

        private void PrepareTasksForReminders()
        {
            // Get tasks with reminders enabled
            var tasks = _context.Tasks
                .Where(t => t.ReminderEnabled && t.DueDate != null)
                .Select(t => new
                {
                    t.Title,
                    t.Description,
                    // Calculate exact reminder time
                    ReminderTime = t.DueDate.Value.AddMinutes(-t.ReminderMinutesBefore)
                })
                .ToList();

            ViewBag.TasksWithReminders = tasks.Select(t => new
            {
                title = t.Title,
                description = t.Description,
                reminderTime = t.ReminderTime.ToString("o") // ISO format for JS
            }).ToList();
        }

    }
}
