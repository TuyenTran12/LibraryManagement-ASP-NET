using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;
using MyWeb.Repositories;
using MyWeb.Services;
using System.Globalization;

namespace MyWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // =========================================================
            //  PHẦN 1: ĐĂNG KÝ DỊCH VỤ (SERVICES)
            // =========================================================

            // 1. Database & Identity
            builder.Services.AddDbContext<MyAppContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<MyAppContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
                options.LoginPath = "/Account/Login";
            });

            // 2. Repositories
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();
            builder.Services.AddScoped<IChapterRepository, ChapterRepository>();

            // 3. CẤU HÌNH VIEW LOCALIZATION (QUAN TRỌNG)

            // Chỉ định thư mục chứa Resource là "Resources"
            // Hệ thống sẽ tìm file theo quy tắc: Resources/Views/ControllerName/ViewName.vi-VN.resx
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews()
                // Kích hoạt IViewLocalizer
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                // Kích hoạt dịch thông báo lỗi (DataAnnotations) trong Model
                .AddDataAnnotationsLocalization();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<Users>>();

                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!roleManager.RoleExistsAsync(roleName).Result)
                    {
                        roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
                    }
                }
                var adminEmail = "admin@mylibrary.com";
                var adminUser = userManager.FindByEmailAsync(adminEmail).Result;

                if (adminUser == null)
                {
                    var newAdmin = new Users
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "System Admin",
                        EmailConfirmed = true
                    };

                    var createAdmin = userManager.CreateAsync(newAdmin, "Admin@123").Result;
                    if (createAdmin.Succeeded)
                    {
                        userManager.AddToRoleAsync(newAdmin, "Admin").Wait();
                    }
                }
            }

            // =========================================================
            //  PHẦN 2: MIDDLEWARE PIPELINE
            // =========================================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // 4. KÍCH HOẠT ĐA NGÔN NGỮ
            var supportedCultures = new[] { "en", "vi" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("vi")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            // 5. Routing & Auth
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Books}/{action=Index}/{id?}");

            app.Run();
        }
    }
}