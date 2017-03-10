using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ideas.Models;
using ideas.Services;
using Newtonsoft.Json;
using MySQL.Data.Entity.Extensions;

namespace ideas
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<IdeasContext>(options =>options.UseMySQL(Configuration.GetConnectionString("IdeasConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<IdeasContext>()
                .AddDefaultTokenProviders();
        
            services.AddMvc();
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

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

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "feed",
                   template: "timeline/",
                   defaults: new { controller = "Home", action = "Feed" }
                   );

                routes.MapRoute(
                  name: "all",
                  template: "all/",
                  defaults: new { controller = "Home", action = "Index" }
                  );

                routes.MapRoute(
                   name: "user",
                   template: "user/{user}",
                   defaults: new { controller = "Profile", action = "User" }
                   );

                routes.MapRoute(
                  name: "login",
                  template: "login",
                  defaults: new { controller = "Account", action = "Login" }
                  );

                  routes.MapRoute(
                      name: "register",
                      template: "register",
                      defaults: new { controller = "Account", action = "Register" }
                  );


                routes.MapRoute(
                    name: "get-details",
                    template: "p/{id}/{name}",
                    defaults: new { controller = "Home" , action = "Details"}
                        );

                routes.MapRoute(
                  name: "create",
                  template: "new",
                  defaults: new { controller = "Home", action = "Create" }
                      );

                routes.MapRoute(
                      name: "updatelist",
                      template: "list",
                      defaults: new { controller = "Home", action = "Update" }
                          );

                routes.MapRoute(
                     name: "update2",
                     template: "update/{id}",
                     defaults: new { controller = "Home", action = "UpdateProcess" }
                         );


                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=feed}");
            });
        }
    }
}
