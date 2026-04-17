using Microsoft.AspNetCore.Mvc;
using TooDooList.Models;
using TooDooList.Services;
using System.ComponentModel.DataAnnotations;

namespace TooDooList.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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
        public async Task<IActionResult> Create(CreateBookingViewModel model)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            bool isCreated = await _bookingService.CreateBookingAsync(
                userId.Value,
                model.ServiceName,
                model.ServiceType,
                model.Description,
                model.ScheduledDate,
                model.Address,
                model.Price
            );

            if (isCreated)
            {
                TempData["SuccessMessage"] = "Booking created successfully!";
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Error creating booking.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var bookings = await _bookingService.GetUserBookingsAsync(userId.Value);
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != userId)
                return NotFound();

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != userId)
                return NotFound();

            // Prevent updating cancelled bookings
            if (booking.Status == BookingStatus.Cancelled)
            {
                TempData["ErrorMessage"] = "Cancelled bookings cannot be updated.";
                return RedirectToAction("Details", new { id = id });
            }

            var model = new UpdateBookingViewModel
            {
                Id = booking.Id,
                ServiceName = booking.ServiceName,
                ServiceType = booking.ServiceType,
                Description = booking.Description,
                ScheduledDate = booking.ScheduledDate,
                Address = booking.Address,
                Price = booking.Price,
                Status = booking.Status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateBookingViewModel model)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Verify booking belongs to user
            var booking = await _bookingService.GetBookingByIdAsync(model.Id);
            if (booking == null || booking.UserId != userId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _bookingService.UpdateBookingAsync(
                model.Id,
                model.ServiceName,
                model.ServiceType,
                model.Description,
                model.ScheduledDate,
                model.Address,
                model.Price,
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

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Verify booking belongs to user
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != userId)
                return NotFound();

            var (success, message) = await _bookingService.UpdateBookingStatusAsync(id, BookingStatus.Cancelled);

            if (success)
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("Details", new { id = id });
        }
    }

    public class CreateBookingViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public ServiceType ServiceType { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }

    public class UpdateBookingViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public ServiceType ServiceType { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Start;
    }
}
