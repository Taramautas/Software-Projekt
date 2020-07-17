using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Planner", policy => policy.RequireRole("Planner"));
                options.AddPolicy("Assistant", policy => policy.RequireRole("Assistant"));
                options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
                options.AddPolicy("LoggedIn", policy => policy.RequireRole("Employee", "Assistant", "Planner"));
            });
            
            // Deliver UserManger for each controller constructor
            services.AddTransient<UserManager>();
            
            // Add HTTPContext Accessor to each controller constructor
            services.AddHttpContextAccessor();
            
            // Added Cronjob (Booking mail reminder) - which runs every 1th minute
            services.AddCronJob<CronTest>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/1 * * * *";
            });
            // 
            services.AddCronJob<DistributionService>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                // 02:00 every day
                //c.CronExpression = @"0 02 * * *";
                //Testingpurpose:
                c.CronExpression = @"*/5 * * * *";
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
            // Dont change the order of these accounts, as the simulation depends on the ids of the accounts
            user_dao.Create("Planner", "admin@admin.de", "admin", Role.Planner);
            user_dao.Create("Assistant", "assistant@assistant.de", "assistant", Role.Assistant);
            user_dao.Create("VIP", "vip@vip.de", "vip", Role.VIP);
            user_dao.Create("Guest", "guest@guest.de", "guest", Role.Employee);
            user_dao.Create("Employee", "user@user.de", "user", Role.Employee);

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

            /*
            // Create Infrastructure for testing
            int loc_id = location_dao.Create("Munich", "12345", "addressstreet 5", 0);
            int zone_id = charging_zone_dao.Create(50, location_dao.GetById(loc_id, 0), 0);
            charging_column_dao.Create(1, false, false, charging_zone_dao.GetById(zone_id, 0), 0);
            charging_column_dao.Create(2, true, false, charging_zone_dao.GetById(zone_id, 0), 0);
            charging_column_dao.Create(1, false, true, charging_zone_dao.GetById(zone_id, 0), 0);

            int loc_id2 = location_dao.Create("Augsburg", "54321", "addressstreet 5", 0);
            int zone_id2 = charging_zone_dao.Create(50, location_dao.GetById(loc_id2, 0), 0);
            charging_column_dao.Create(1, false, false, charging_zone_dao.GetById(zone_id2, 0), 0);
            charging_column_dao.Create(2, true, false, charging_zone_dao.GetById(zone_id2, 0), 0);
            charging_column_dao.Create(1, false, true, charging_zone_dao.GetById(zone_id2, 0), 0);
            */ 
            //
            
            
            
            //Vehicle startup
            List<ConnectorType> tmp_conn_types = new List<ConnectorType>();
            tmp_conn_types.Add(ConnectorType.Schuko_Socket);
            vehicle_dao.Create("TestModel",400, tmp_conn_types);
            tmp_conn_types = new List<ConnectorType>();
            tmp_conn_types.Add(ConnectorType.Tesla_Supercharger);
            vehicle_dao.Create("BlaModel", 999, tmp_conn_types);
            tmp_conn_types = new List<ConnectorType>();
            tmp_conn_types.Add(ConnectorType.CHAdeMO_Plug);
            vehicle_dao.Create("MarcinCodeMobile", 20, tmp_conn_types);
            //
            
            //CCTYPE startup 
            ChargingColumnTypeDaoImpl charging_column_type_dao = new ChargingColumnTypeDaoImpl(cache);
            List<Tuple<ConnectorType,int>> connector_list = new List<Tuple<ConnectorType, int>>();
            connector_list.Add(new Tuple<ConnectorType, int>(ConnectorType.Schuko_Socket, 70));
            connector_list.Add(new Tuple<ConnectorType, int>(ConnectorType.Tesla_Supercharger, 60));
            charging_column_type_dao.Create("RadiFast'n Charge", "Rados", 2, connector_list);
            connector_list = new List<Tuple<ConnectorType, int>>();
            connector_list.Add(new Tuple<ConnectorType, int>(ConnectorType.CHAdeMO_Plug, 80));
            charging_column_type_dao.Create("Marcos - ultraspeed", "Marcinos", 1, connector_list);

            //
            
            //Location Startup
            LocationDaoImpl location_dao_ = new LocationDaoImpl(cache);
            location_dao_.Create("Augsburg", "86165", "BeneStreet", 0);
            location_dao_.Create("Berlin", "10033", "Blublaa", 0);
            //
            
            //ChargingZone startup
            ChargingZoneDaoImpl charging_zone_dao_ = new ChargingZoneDaoImpl(cache);
            //Augsburg
            charging_zone_dao_.Create("Alpha", 100, location_dao_.GetById(1,0), 0);
            charging_zone_dao_.Create("Beta", 250, location_dao_.GetById(1,0), 0);
            charging_zone_dao_.Create("Gamma", 400, location_dao_.GetById(1,0), 0);
            //Berlin
            charging_zone_dao_.Create("Omega", 30, location_dao_.GetById(2,0), 0);
            //
            
            //Booking startup
            booking_dao.Create(10, 30, new DateTime(2020, 07, 20, 10, 20, 0), new DateTime(2020, 07, 20, 12, 20, 0),
                vehicle_dao.GetById(1), user_dao.GetById(5), location_dao.GetById(1,0), 0);
            booking_dao.GetById(1, 0).Accept();
            
            //ChargingColumn startup
            ChargingColumnDaoImpl charging_column = new ChargingColumnDaoImpl(cache);
            ChargingColumnTypeDaoImpl charging_type = new ChargingColumnTypeDaoImpl(cache);
            charging_column.Create(charging_type.GetById(1), charging_zone_dao.GetById(1,0),null, 0);
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
                    pattern: "{controller=Home}/{action=LogIn}/{id?}");
            });
        }
    }
}
