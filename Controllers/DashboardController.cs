using Microsoft.AspNetCore.Mvc;
using TooDooList.Services;

namespace TooDooList.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IBookingService _bookingService;

        public DashboardController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var statistics = await _bookingService.GetBookingStatisticsAsync(userId.Value);
            return View(statistics);
        }
    }
}
