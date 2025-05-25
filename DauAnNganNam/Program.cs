using DauAnNganNam.Models;
using Microsoft.EntityFrameworkCore;

namespace DauAnNganNam
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register IHttpClientFactory
            builder.Services.AddHttpClient();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection service, ConfigurationManager configuration)
        {
            service.AddDbContext<ExeDBContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            service.AddControllersWithViews();
        }
    }
}
