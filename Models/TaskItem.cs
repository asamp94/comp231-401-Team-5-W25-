using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flow_App.Models
{
    public class TaskItem
    {
        public int Id { get; set; }            

        [Required]
        public string Title { get; set; } = "";  

        public string? Description { get; set; }      
        
        public DateTime? DueDate { get; set; }        

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

       
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public bool ReminderEnabled { get; set; } = false;
        public int ReminderMinutesBefore { get; set; } = 60; //default of 1 hour

        // Helper property (not mapped to DB) for UI binding
        [NotMapped]
        public int ReminderValue { get; set; } = 60; // default 60
        [NotMapped]
        public string ReminderUnit { get; set; } = "minutes"; // "minutes", "hours", "days"
    }
}
