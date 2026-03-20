using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TooDooList.Models
{
    public enum TaskStatus
    {
        Ongoing,
        Completed,
        Cancelled
    }

    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Ongoing;

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? CompletedDate { get; set; }

        // Navigation property
        public virtual User? User { get; set; }
    }
}
