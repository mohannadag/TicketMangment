using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TicketMangment.Models;
using TicketMangment.SharedClasses;

namespace TicketMangment
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
            services.AddDbContextPool<AppDbContext>(
                options => {
                    options.UseSqlServer(Configuration.GetConnectionString("TicketDBConection"));
                    //options.EnableSensitiveDataLogging(true);      <-- this for enabling sensitive data loging
                    });
            // this for enabling sensitive data loging too
            //services.AddLogging(loggingBuilder => {
            //    loggingBuilder.AddConsole()
            //        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
            //    loggingBuilder.AddDebug();
            //});

            services.AddIdentity<ApplicationUser, IdentityRole>(options => { 
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ "; })
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddControllersWithViews(config => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                });

            services.ConfigureApplicationCookie(options =>
            {
                // we can change the path of AccessDeniedPath here
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
                // and we can change many things too like the login path  options.LoginPath
            });

            // Claim Policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                // we can add many claim by adding .RequireClaim("claim name") at the end (method chaining)

                // we used the claim value here to make sure just the users with edit role and value true can auth
                // options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "true"));
                options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role"));

                options.AddPolicy("ManageClaimsRolePolicy", policy => policy.RequireClaim("Manage Claims Role"));
                options.AddPolicy("ManageClaimsUserPolicy", policy => policy.RequireClaim("Manage Claims User"));
                options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role"));
                options.AddPolicy("UsersListPolicy", policy => policy.RequireClaim("Users List"));
                options.AddPolicy("EditUserPolicy", policy => policy.RequireClaim("Edit User"));
                options.AddPolicy("MangeUserRolesPolicy", policy => policy.RequireClaim("Mange User Roles"));
                options.AddPolicy("DeleteUserPolicy", policy => policy.RequireClaim("Delete User"));
                options.AddPolicy("ListRolesPolicy", policy => policy.RequireClaim("List Roles"));
                options.AddPolicy("EditUsersInRolePolicy", policy => policy.RequireClaim("Edit Users In Role"));
                options.AddPolicy("AccessControlPanelPolicy", policy => policy.RequireClaim("Access Control Panel"));
            });

            // Role Policy
            services.AddAuthorization(options =>
            {
                // we can add as many roles as we want by adding comma betwen the roles
                // like this .RequireRole("Admin", "User"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
            });

            services.AddScoped<ITicketRepo, SQLTicketRepo>();
            services.AddScoped<IDepartmentRepo, SQLDepartmentRepo>();
            services.AddScoped<IPriorityRepo, SQLPriorityRepo>();
            services.AddScoped<ICompanyRepo, SQLCompanyRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
