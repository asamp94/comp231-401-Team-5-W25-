using Flow_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Flow_App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddDbContext<FlowContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FlowContext")));

           
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

           
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

           
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Tasks}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
