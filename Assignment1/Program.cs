using Assignment1.Data;
using Assignment1.Hubs;
using Assignment1.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Assignment1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // wire up dbcontext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton(x =>
                new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));

            builder.Services.AddScoped<BlobStorageService>();

            builder.Services
                .AddDefaultIdentity<IdentityUser>(options => {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // use signalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await DbInitializer.Initialize(scope.ServiceProvider);
            }

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // map hub to endpoint
            app.MapHub<AttendeeHub>("/attendeeHub");

            app.MapRazorPages();
            app.Run();
        }
    }
}
