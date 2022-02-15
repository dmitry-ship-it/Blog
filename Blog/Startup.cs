using Blog.DAL.Repositories;
using Blog.Data;
using Blog.Data.DbModels;
using Blog.Data.Validation;
using Blog.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

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

            #region Repositories

            services.AddScoped<Repository<Article>, ArticleRepository>(x =>
                new ArticleRepository(x.GetRequiredService<ApplicationDbContext>()));

            services.AddScoped<Repository<Comment>, CommentRepository>(x =>
                new CommentRepository(x.GetRequiredService<ApplicationDbContext>()));

            services.AddScoped<Repository<User>, UserRepository>(x =>
                new UserRepository(x.GetRequiredService<ApplicationDbContext>()));

            #endregion

            services.AddControllersWithViews();
            services.AddRazorPages();

            #region JWToken

            SiteKeys.Configure(Configuration.GetSection("AppSettings"));
            var key = Encoding.ASCII.GetBytes(SiteKeys.Token);

            services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(60));
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(token =>
            {
                token.RequireHttpsMetadata = false;
                token.SaveToken = true;
                token.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = SiteKeys.WebSiteDomain,
                    ValidateAudience = true,
                    ValidAudience = SiteKeys.WebSiteDomain,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion
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

            #region JWToken

            app.UseCookiePolicy();
            app.UseSession();
            app.Use(async (context, next) =>
            {
                var JWToken = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(JWToken))
                {
                    context.Request.Headers.Add("Authorization", $"Bearer {JWToken}");
                }

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // Add default roles and default admin
            // for application testing
            AddDefaultUser(serviceProvider);
        }

        private void AddDefaultUser(IServiceProvider serviceProvider)
        {
            var userRepository = serviceProvider.GetRequiredService<Repository<User>>();

            // Add default admin
            // to the Administrator role
            const string username = "John_Doe";
            const string password = "_AStrongP@ssword1!";

            var testUser = userRepository.GetAsync(user => user.Username == username);
            testUser.Wait();

            // if there no user with specified userName  
            if (testUser.Result is null)
            {
                var defaultAdmin = UserProtector.CreateUser(username, password, Role.Admin);

                userRepository.InsertAsync(defaultAdmin).Wait();
                userRepository.SaveAsync().Wait();
            }
        }
    }
}
