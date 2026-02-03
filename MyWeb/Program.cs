using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWeb.Data;
using MyWeb.Models;

namespace MyWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Tạo trình xây dựng ứng dụng web với các tham số dòng lệnh
            var builder = WebApplication.CreateBuilder(args);           

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<MyAppContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }
            ).AddEntityFrameworkStores<MyAppContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14); 
                options.SlidingExpiration = true;
                options.LoginPath = "/Account/Login";
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Chuyển hướng các yêu cầu HTTP sang HTTPS
            app.UseHttpsRedirection();
            // Xác định các tuyến đường cho các yêu cầu HTTP
            app.UseRouting();

            // Xác thực và ủy quyền người dùng
            app.UseAuthorization();
            // Ánh xạ các tài nguyên tĩnh như CSS, JS, hình ảnh
            app.MapStaticAssets();

            // Ánh xạ các tuyến đường điều khiển với tài nguyên tĩnh
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Book}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Chạy ứng dụng web
            app.Run();           
        }
    }
}
