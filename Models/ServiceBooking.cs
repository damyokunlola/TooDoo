using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TooDooList.Models
{
    public enum ServiceType
    {
        Laundry,
        DryCleaning,
        DeepCleaning,
        SofaCleaning
    }

    public enum BookingStatus
    {
        Start,
        InProgress,
        Completed,
        Cancelled
    }

    public class ServiceBooking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public ServiceType ServiceType { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Start;

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? CompletedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        // Navigation property
        public virtual User? User { get; set; }
    }
}
