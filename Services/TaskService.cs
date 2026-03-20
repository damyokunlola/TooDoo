using TooDooList.Data;
using TooDooList.Models;
using Microsoft.EntityFrameworkCore;

namespace TooDooList.Services
{
    public interface ITaskService
    {
        Task<TaskStatistics> GetTaskStatisticsAsync(int userId);
        Task<List<TaskItem>> GetUserTasksAsync(int userId);
        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<bool> CreateTaskAsync(int userId, string title, string description, Models.TaskStatus status);
        Task<(bool success, string message)> UpdateTaskStatusAsync(int taskId, Models.TaskStatus status);
        Task<(bool success, string message)> UpdateTaskAsync(int taskId, string title, string description, Models.TaskStatus status);
    }

    public class TaskStatistics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OngoingTasks { get; set; }
        public int CancelledTasks { get; set; }
    }

    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public TaskService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<TaskStatistics> GetTaskStatisticsAsync(int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return new TaskStatistics
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == Models.TaskStatus.Completed),
                OngoingTasks = tasks.Count(t => t.Status == Models.TaskStatus.Ongoing),
                CancelledTasks = tasks.Count(t => t.Status == Models.TaskStatus.Cancelled)
            };
        }

        public async Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> CreateTaskAsync(int userId, string title, string description, Models.TaskStatus status)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            var task = new TaskItem
            {
                Title = title,
                Description = description,
                Status = status,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Send task created email (fire and forget)
            _ = _emailService.SendTaskCreatedEmailAsync(user.Email, user.Name, title);

            return true;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            return await _context.Tasks.FindAsync(taskId);
        }

        public async Task<(bool success, string message)> UpdateTaskStatusAsync(int taskId, Models.TaskStatus status)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return (false, "Task not found.");

            // Prevent updating completed tasks
            if (task.Status == Models.TaskStatus.Completed)
                return (false, "Completed tasks cannot be updated.");

            task.Status = status;
            if (status == Models.TaskStatus.Completed)
                task.CompletedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Task updated successfully.");
        }

        public async Task<(bool success, string message)> UpdateTaskAsync(int taskId, string title, string description, Models.TaskStatus status)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return (false, "Task not found.");

            // Prevent updating completed tasks
            if (task.Status == Models.TaskStatus.Completed)
                return (false, "Completed tasks cannot be updated.");

            task.Title = title;
            task.Description = description;
            task.Status = status;

            if (status == Models.TaskStatus.Completed)
                task.CompletedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Task updated successfully.");
        }
    }
}
