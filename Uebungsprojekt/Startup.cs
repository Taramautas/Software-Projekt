using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.OccupancyPlans;
using Uebungsprojekt.Service;

namespace Uebungsprojekt
{
    /// <summary>
    /// Configure Services running in the background
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services object to activate custom services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC functionality
            services.AddControllersWithViews();
            
            // Add cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config => {
                    config.LoginPath = "/Home/Login/";
                    config.LogoutPath = "/Home/Logout/";
                    config.AccessDeniedPath = "/Home/Error/";
                });
            
            // Deliver UserManger for each controller constructor
            services.AddTransient<UserManager>();
            
            // Add HTTPContext Accessor to each controller constructor
            services.AddHttpContextAccessor();
            
            // Added Cronjob - which runs every 15th minute
            services.AddCronJob<CronTest>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/1 * * * *";
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// Here you specify the pipeline each request has to go through before being directed to a controller.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache cache)
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            booking_dao.GetAll(0);
            VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
            vehicle_dao.GetAll();
            UserDao user_dao = new UserDaoImpl(cache);
            user_dao.GetAll();
            user_dao.Create("Admin", "admin@admin.de", "admin", Role.Planner);
            
            SimulationConfigDao config_dao = new SimulationConfigDaoImpl(cache);
            config_dao.GetAll();
            SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
            infrastructure_dao.GetAll();
            SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
            result_dao.GetAll();
            
            LocationDao location_dao = new LocationDaoImpl(cache);
            location_dao.GetAll(0);
            ChargingZoneDao charging_zone_dao = new ChargingZoneDaoImpl(cache);
            charging_zone_dao.GetAll(0);
            ChargingColumnDao charging_column_dao = new ChargingColumnDaoImpl(cache);
            charging_column_dao.GetAll(0);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseWebSockets();

            app.UseRouting();

            // Add 
            app.UseAuthentication();
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
