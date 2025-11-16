using System.Collections.Generic;

namespace Flow_App.Models
{
    public class Course
    {
        public int Id { get; set; }                  
        public string Name { get; set; } = "";       

        
        public List<TaskItem> Tasks { get; set; } = new();
    }
}
