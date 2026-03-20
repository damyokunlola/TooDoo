using Microsoft.AspNetCore.Mvc;
using TooDooList.Models;
using TooDooList.Services;
using System.ComponentModel.DataAnnotations;

namespace TooDooList.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            bool isCreated = await _taskService.CreateTaskAsync(
                userId.Value,
                model.Title,
                model.Description,
                model.Status
            );

            if (isCreated)
            {
                TempData["SuccessMessage"] = "Task created successfully!";
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Error creating task.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyTasks()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tasks = await _taskService.GetUserTasksAsync(userId.Value);
            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null || task.UserId != userId)
                return NotFound();

            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null || task.UserId != userId)
                return NotFound();

            // Prevent updating completed tasks
            if (task.Status == Models.TaskStatus.Completed)
            {
                TempData["ErrorMessage"] = "Completed tasks cannot be updated.";
                return RedirectToAction("Details", new { id = id });
            }

            var model = new UpdateTaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateTaskViewModel model)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Verify task belongs to user
            var task = await _taskService.GetTaskByIdAsync(model.Id);
            if (task == null || task.UserId != userId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _taskService.UpdateTaskAsync(
                model.Id,
                model.Title,
                model.Description,
                model.Status
            );

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction("Details", new { id = model.Id });
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }
    }

    public class CreateTaskViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Models.TaskStatus Status { get; set; } = Models.TaskStatus.Ongoing;
    }

    public class UpdateTaskViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Models.TaskStatus Status { get; set; } = Models.TaskStatus.Ongoing;
    }
}
