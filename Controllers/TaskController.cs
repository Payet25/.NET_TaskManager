using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerApp.Models;
using AppTask = TaskManagerApp.Models.Task; // Alias to avoid ambiguity

namespace TaskManagerApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            if (_context.Tasks == null)
            {
                ViewBag.CompletionPercentage = 0;
                ViewBag.TotalPages = 1;
                ViewBag.CurrentPage = 1;
                return View(Enumerable.Empty<AppTask>());
            }

            var tasks = await _context.Tasks.ToListAsync();

            if (!tasks.Any())
            {
                ViewBag.CompletionPercentage = 0;
                ViewBag.TotalPages = 1;
                ViewBag.CurrentPage = 1;
                return View(tasks);
            }

            int completedCount = tasks.Count(t => t.IsCompleted);
            int totalCount = tasks.Count();
            int completionPercentage = (totalCount == 0) ? 0 : (completedCount * 100 / totalCount);

            ViewBag.CompletionPercentage = completionPercentage;

            var paginatedTasks = tasks.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            ViewBag.TotalPages = (int)Math.Ceiling((double)tasks.Count() / pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(paginatedTasks);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppTask task)
        {
            if (ModelState.IsValid)
            {
                _context.Tasks.Add(task);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        public IActionResult ExportToCsv()
        {
            var tasks = _context.Tasks.ToList();
            var csv = new StringBuilder();
            csv.AppendLine("Title,Description,DueDate,Priority,Status");

            foreach (var task in tasks)
            {
                csv.AppendLine($"{task.Title},{task.Description},{task.DueDate.ToShortDateString()},{task.Priority},{(task.IsCompleted ? "Completed" : "Pending")}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "Tasks.csv");
        }

        public IActionResult Edit(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost]
        public IActionResult Edit(AppTask task)
        {
            if (ModelState.IsValid)
            {
                _context.Tasks.Update(task);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        public IActionResult Delete(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
