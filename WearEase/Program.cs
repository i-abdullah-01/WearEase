using WearEase.Hubs;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WearEase.Data;
using WearEase.Models.Interfaces;
using WearEase.Models.Repositories;
using WearEase.Models.Services;

namespace WearEase
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //ADD CLAIM/POLICY AUTHORIZATION

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("Role", "Admin"));

                options.AddPolicy("ProductManager", policy =>
                    policy.RequireClaim("Permission", "ManageProducts"));

                options.AddPolicy("CategoryManager", policy =>
                    policy.RequireClaim("Permission", "ManageCategories"));

                options.AddPolicy("OrderManager", policy =>
                    policy.RequireClaim("Permission", "ManageOrders"));


                //  USER ONLY (NOT ADMIN)
                options.AddPolicy("UserOnly", policy =>
                    policy.RequireAssertion(context =>
                        !context.User.HasClaim("Role", "Admin")));
            });


            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();


            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<OrderService>();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".WearEase.Session";
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();

           builder.Services.AddSignalR();



            var app = builder.Build();
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    

                    context.Response.StatusCode = 500;
                    context.Response.Redirect("/Home/Error");
                }
            });

            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();


          ;   
            app.UseRouting();
            app.UseSession();
           
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=WearEase}/{action=Index}/{id?}");
            app.MapRazorPages();
           
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                string adminEmail = "admin@wearease.com";
                string adminPassword = "Admin@123";

                var admin = await userManager.FindByEmailAsync(adminEmail);

                if (admin == null)
                {
                    admin = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddClaimAsync(admin, new Claim("Role", "Admin"));
                        await userManager.AddClaimAsync(admin, new Claim("Permission", "ManageProducts"));
                        await userManager.AddClaimAsync(admin, new Claim("Permission", "ManageOrders"));
                        await userManager.AddClaimAsync(admin, new Claim("Permission", "ManageCategories")); //<-- FIX
                    }
                }
            }

            app.MapHub<OrderHub>("/orderHub");

            app.Run();
            

        }
    }
}
