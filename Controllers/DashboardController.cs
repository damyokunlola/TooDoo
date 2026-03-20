using Microsoft.AspNetCore.Mvc;
using TooDooList.Services;

namespace TooDooList.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ITaskService _taskService;

        public DashboardController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var statistics = await _taskService.GetTaskStatisticsAsync(userId.Value);
            return View(statistics);
        }
    }
}
