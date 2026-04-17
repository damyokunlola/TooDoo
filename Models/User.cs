using System.ComponentModel.DataAnnotations;

namespace TooDooList.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(100)]
        public string Profession { get; set; } = string.Empty;

        [Required]
        public string EncryptedPassword { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<TaskItem>? Tasks { get; set; }
        public virtual ICollection<ServiceBooking>? ServiceBookings { get; set; }
    }
}
