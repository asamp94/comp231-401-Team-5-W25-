using System;
using System.ComponentModel.DataAnnotations;

namespace Flow_App.Models
{
    public class TaskItem
    {
        public int Id { get; set; }            

        [Required]
        public string Title { get; set; } = "";  

        public string? Description { get; set; }      
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }        

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

       
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
