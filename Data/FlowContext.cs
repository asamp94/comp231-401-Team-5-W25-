using Flow_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Flow_App.Data

    /*  This is the database connection and what tables in the database exist. 
     * 
     */
{
    public class FlowContext : DbContext
    {
        public FlowContext(DbContextOptions<FlowContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
