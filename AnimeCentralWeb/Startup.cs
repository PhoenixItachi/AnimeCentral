using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AnimeCentralWeb.Data;
using AnimeCentralWeb.Models;
using AnimeCentralWeb.Services;
using AutoMapper;
using AnimeCentralWeb.AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace AnimeCentralWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<AnimeCentralDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<AnimeCentralDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1073741824;
            });

            services.AddMvc();
            services.AddSignalR();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<UserManager<ApplicationUser>>();
            // Configure AutoMapper
            Mapper.Initialize(c => c.AddProfiles(new[] { typeof(ModelViewToDomainConfigurationProfile), typeof(DomainToModelViewConfigurationProfile) }));

            // Configure SendGrid
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSignalR();
            var serviceProvider = app.ApplicationServices;
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()) {

                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var context = scope.ServiceProvider.GetService<AnimeCentralDbContext>();

                await context.Database.MigrateAsync();
                await SetUserRolesAndDefaultUser(context, userManager, roleManager);
            }
                
        }

        private async Task SetUserRolesAndDefaultUser(AnimeCentralDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminExists)
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                await roleManager.CreateAsync(role);

                var user = new ApplicationUser();
                user.UserName = "AdminAC";
                user.Email = "admin@animecentral.com";

                string userPWD = "AdminAC123;";

                var chkUser = await userManager.CreateAsync(user, userPWD);
                if (chkUser.Succeeded)
                    await userManager.AddToRoleAsync(user, "Admin");
                else
                    throw new Exception(String.Join(Environment.NewLine, chkUser.Errors.Select(x => $"Code{x.Code}: {x.Description}")));
            }

            var userExists = await roleManager.RoleExistsAsync("User");
            if (!userExists)
            {
                var role = new IdentityRole();
                role.Name = "User";
                await roleManager.CreateAsync(role);
            }
        }
    }
}
