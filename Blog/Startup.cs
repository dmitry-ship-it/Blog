using Blog.DAL.Interfaces;
using Blog.DAL.Repositories;
using Blog.Data;
using Blog.Data.DatabaseModels;
using Blog.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Directory.GetCurrentDirectory();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                        .Replace("[DataDirectory]", dbPath)));

            services.AddScoped<IRepository<Article>, ArticleRepository>(x =>
                new ArticleRepository(x.GetRequiredService<ApplicationDbContext>()));

            services.AddScoped<IRepository<Comment>, CommentRepository>(x =>
                new CommentRepository(x.GetRequiredService<ApplicationDbContext>()));

            services.AddScoped<IRepository<User>, UserRepository>(x =>
                new UserRepository(x.GetRequiredService<ApplicationDbContext>()));

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // Add default roles and default admin
            // for application testing
            if (env.IsDevelopment())
            {
                CreateRoles(serviceProvider);
            }
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            // Task<IdentityResult> roleResult;

            //Check if roles exist
            /*
            var roles = new string[] { "Administrator", "User" };

            foreach (var role in roles)
            {
                var roleExists = roleManager.RoleExistsAsync(role);
                roleExists.Wait();

                if (!roleExists.Result)
                {
                    roleResult = roleManager.CreateAsync(new IdentityRole(role));
                    roleResult.Wait();
                }
            }
            */

            // Add default admin
            // to the Administrator role
            const string userName = "John_Doe";
            // const string email = "someone@somewhere.com";
            const string password = "_AStrongP@ssword1!";

            var users = userRepository.GetAllAsync();
            users.Wait();

            var testUser = users.Result.SingleOrDefault(user => user.Username == userName);

            if (testUser is null)
            {
                var defaultAdmin = new User
                {
                    Username = userName,
                    Password = password
                };

                userRepository.InsertAsync(defaultAdmin);

                //if (newUser)
                //{
                //    var newUserRole = userRepository.AddToRoleAsync(defaultAdmin, "Administrator");
                //    newUserRole.Wait();
                //}
            }
        }
    }
}
