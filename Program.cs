using ClinicalManagementSystem2025.Repository;
using ClinicalManagementSystem2025.Services;
using Microsoft.AspNetCore.Http;

namespace ClinicalManagementSystem2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // Add repositories and services
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();

            // Add Configuration for connection string
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

            // Session configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // Show detailed errors in development
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add session middleware
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "doctor",
                pattern: "Doctor/{action}/{patientId?}/{appointmentId?}",
                defaults: new { controller = "Doctor" });

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Doctor}/{action=Dashboard}/{id?}");

            app.Run();
        }
    }
}