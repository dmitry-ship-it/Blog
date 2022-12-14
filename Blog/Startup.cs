using Blog.DAL.Repositories;
using Blog.Data;
using Blog.Data.DbModels;
using Blog.Data.Validation;
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
            #region Database configuration (incl. connection string)

            var dbPath = Directory.GetCurrentDirectory();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                        .Replace("[DataDirectory]", dbPath)));

            #endregion

            #region Repositories

            services.AddScoped<Repository<Article>, ArticleRepository>();

            services.AddScoped<Repository<Comment>, CommentRepository>();

            services.AddScoped<Repository<User>, UserRepository>();

            services.AddScoped<UserManager>();

            #endregion

            #region Add controllers views and razor pages

            services.AddControllersWithViews();
            services.AddRazorPages();

            #endregion

            #region JWToken

            SiteKeys.Configure(Configuration.GetSection("AppSettings"));
            var key = Encoding.UTF8.GetBytes(SiteKeys.Token);

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
            #region Default configuration (error page, static files, https, status code pages)

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

            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;
                response.Redirect($"/Error/{response.StatusCode}");
                return Task.CompletedTask;
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            #endregion

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

            #region Endpoints

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            #endregion

            // Add default admin
            // for application testing
            AddDefaultUser(serviceProvider);
        }

        private void AddDefaultUser(IServiceProvider serviceProvider)
        {
            var userRepository = serviceProvider.GetRequiredService<Repository<User>>();
            var userManager = serviceProvider.GetRequiredService<UserManager>();

            // Add default admin
            // to the 'Admin' role
            const string username = "John_Doe";
            const string password = "_AStrongP@ssword1!";

            var testUser = userRepository.GetAsync(user => user.Username == username);
            testUser.Wait();

            // if there no user with specified username  
            if (testUser.Result is null)
            {
                var defaultAdmin = userManager.CreateUser(username, password, Role.Admin);

                userRepository.InsertAsync(defaultAdmin).Wait();
            }
        }
    }
}
