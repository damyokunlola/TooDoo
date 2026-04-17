using TooDooList.Data;
using TooDooList.Models;
using Microsoft.EntityFrameworkCore;

namespace TooDooList.Services
{
    public interface IBookingService
    {
        Task<BookingStatistics> GetBookingStatisticsAsync(int userId);
        Task<List<ServiceBooking>> GetUserBookingsAsync(int userId);
        Task<ServiceBooking?> GetBookingByIdAsync(int bookingId);
        Task<bool> CreateBookingAsync(int userId, string serviceName, ServiceType serviceType, 
            string description, DateTime scheduledDate, string address, decimal price);
        Task<(bool success, string message)> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<(bool success, string message)> UpdateBookingAsync(int bookingId, string serviceName, 
            ServiceType serviceType, string description, DateTime scheduledDate, string address, decimal price, BookingStatus status);
    }

    public class BookingStatistics
    {
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int InProgressBookings { get; set; }
        public int StartBookings { get; set; }
        public int CancelledBookings { get; set; }
    }

    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public BookingService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<BookingStatistics> GetBookingStatisticsAsync(int userId)
        {
            var bookings = await _context.ServiceBookings
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return new BookingStatistics
            {
                TotalBookings = bookings.Count,
                CompletedBookings = bookings.Count(b => b.Status == BookingStatus.Completed),
                InProgressBookings = bookings.Count(b => b.Status == BookingStatus.InProgress),
                StartBookings = bookings.Count(b => b.Status == BookingStatus.Start),
                CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled)
            };
        }

        public async Task<List<ServiceBooking>> GetUserBookingsAsync(int userId)
        {
            return await _context.ServiceBookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.ScheduledDate)
                .ToListAsync();
        }

        public async Task<bool> CreateBookingAsync(int userId, string serviceName, ServiceType serviceType,
            string description, DateTime scheduledDate, string address, decimal price)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            var booking = new ServiceBooking
            {
                ServiceName = serviceName,
                ServiceType = serviceType,
                Description = description,
                ScheduledDate = scheduledDate,
                Address = address,
                Price = price,
                Status = BookingStatus.Start,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.ServiceBookings.Add(booking);
            await _context.SaveChangesAsync();

            // Send booking created email (fire and forget)
            _ = _emailService.SendBookingCreatedEmailAsync(user.Email, user.Name, serviceName);

            return true;
        }

        public async Task<ServiceBooking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.ServiceBookings.FindAsync(bookingId);
        }

        public async Task<(bool success, string message)> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            var booking = await _context.ServiceBookings.FindAsync(bookingId);
            if (booking == null)
                return (false, "Booking not found.");

            // Prevent updating cancelled bookings
            if (booking.Status == BookingStatus.Cancelled)
                return (false, "Cancelled bookings cannot be updated.");

            booking.Status = status;
            if (status == BookingStatus.Completed)
                booking.CompletedDate = DateTime.UtcNow;
            else if (status == BookingStatus.Cancelled)
                booking.CancelledDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Booking updated successfully.");
        }

        public async Task<(bool success, string message)> UpdateBookingAsync(int bookingId, string serviceName,
            ServiceType serviceType, string description, DateTime scheduledDate, string address, decimal price, BookingStatus status)
        {
            var booking = await _context.ServiceBookings.FindAsync(bookingId);
            if (booking == null)
                return (false, "Booking not found.");

            // Prevent updating cancelled bookings
            if (booking.Status == BookingStatus.Cancelled)
                return (false, "Cancelled bookings cannot be updated.");

            booking.ServiceName = serviceName;
            booking.ServiceType = serviceType;
            booking.Description = description;
            booking.ScheduledDate = scheduledDate;
            booking.Address = address;
            booking.Price = price;
            booking.Status = status;

            if (status == BookingStatus.Completed)
                booking.CompletedDate = DateTime.UtcNow;
            else if (status == BookingStatus.Cancelled)
                booking.CancelledDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Booking updated successfully.");
        }
    }
}
