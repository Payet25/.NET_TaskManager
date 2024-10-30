using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        [Required]
        public string Priority { get; set; } = "Low";

        public bool IsCompleted { get; set; } = false;
    }
}
