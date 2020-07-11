﻿using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Simulations;
using Uebungsprojekt.ViewModel.Administration;

namespace Uebungsprojekt.Controllers
{

    [Authorize(Roles = "Planner")]
    public class AdministrationController : Controller
    {
        private readonly UserManager user_manager;
        private int user_id;
        
        private readonly int max_allowed_filesize = (1024 * 1024) * 1; // Last multiplicator = mb
        private IMemoryCache cache; // TODO: Evaluation


        /// <summary>
        /// Constructor for AdministrationController
        /// </summary>
        public AdministrationController(UserManager user_manager, IHttpContextAccessor http_context_accessor, IMemoryCache cache)
        {
            this.cache = cache;
            this.user_manager = user_manager;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
        }

        /// <summary>
        /// General administration overview
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Show View containing the form for simulation parameters and infrastructure
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public IActionResult SimulationConfig()
        {
            return View(new SimulationConfig());
        }
        
        /// <summary>
        /// Show View containing the form for simulation parameters and infrastructure
        /// </summary>
        /// <returns>View</returns>
        [HttpPost]
        public IActionResult SimulationConfig(SimulationConfig config)
        {
            SimulationConfigDao config_dao = new SimulationConfigDaoImpl(cache);
            int config_id = config_dao.Create(config.tick_minutes, config.rush_hours, config.min, config.max, config.spread, config.weeks, config.vehicles);
            return RedirectToAction("SimulationInfrastructure", config.id);
        }
        
        /// <summary>
        /// Passes both the simulation infrastructure and configuration to the simulation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SimulationInfrastructure(int simulation_config_id)
        {
             SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
             SimulationInfrastructureViewModel view_model = new SimulationInfrastructureViewModel();
             // ChargingColumnTypeDao type_dao = new ChargingColumnTypeDaoImpl(cache) TODO: 
             view_model.all_simulation_infrastructures = infrastructure_dao.GetAll();
             view_model.simulation_config_id = simulation_config_id;
             //view_model.charging_column_types = type_dao.GetAll();
             return View(view_model);
        }
        
        
        

        /// <summary>
        /// Passes both the simulation infrastructure and configuration to the simulation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SimulationInfrastructure(SimulationInfrastructureViewModel view_model)
        {
            /* TODO:
             * SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
             * int infrastructure_id = infrastructure_dao.Create(config.tick_minutes, config.rush_hours, config.min, config.max, config.spread, config.weeks, config.vehicles);
             */
            int infrastructure_id = 0;
            SimulationViewModel simulation_view_model = new SimulationViewModel()
            {
                simulation_config_id = view_model.simulation_config_id,
                simulation_infrastructure_id = infrastructure_id,
            };
            return Simulation(simulation_view_model);
        }
        
        [HttpGet]
        public IActionResult Simulation()
        {
            return RedirectToAction("SimulationConfig");
        }


        /// <summary>
        /// Start and visualize the actual simulation
        /// </summary>
        /// <param name="view_model"></param>
        [HttpPost]
        public IActionResult Simulation(SimulationViewModel view_model)
        {
            /* TODO: 
             * SimulationConfigDao config_dao = new SimulationConfigDaoImpl(cache);
             * SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
             * SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
             * SimulationConfig config = config_dao.GetById(view_mode.simulation_config_id);
             * SimulationInfrastructure infrastructure = infrastructure_dao.GetById(view_mode.simulation_infrastructure_id);
             * SimulationResult result = result_dao.Create(config.id, infrastructure.id);
             */
            
            SimulationConfig config = new SimulationConfig();
            SimulationInfrastructure infrastructure = new SimulationInfrastructure();
            SimulationResult result = new SimulationResult()
            {
                config_id = view_model.simulation_config_id,
                infrastructure_id = view_model.simulation_infrastructure_id
            };
            
            
            Simulation simulation = new Simulation(config, infrastructure, result);
            if (!simulation.RunSimulation())
            {
                Console.Out.WriteLine("Failure on simulation");
                return RedirectToPage("/Home/Error/");
            }
            return View(simulation.simulation_result.id);
        }

        [HttpPost]
        public async Task GetSimulationResults(int simulation_result_id)
        {
            /* TODO:
             * SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
             * SimulationResult result = result_dao.GetByID(simulation_result_id);
             */
            SimulationResult result = new SimulationResult();
            
            var context = ControllerContext.HttpContext;
            var is_socket_request = context.WebSockets.IsWebSocketRequest;

            if (is_socket_request)
            {
                WebSocket web_socket = await context.WebSockets.AcceptWebSocketAsync();
                await SendSimulationUpdates(context, web_socket, result);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task SendSimulationUpdates(HttpContext context, WebSocket web_socket, SimulationResult result)
        {
            List<string> messages = new List<string>();
            if (result == null || 
                result.num_generated_bookings.Count != result.total_workload.Count || 
                result.total_workload.Count != result.unsatisfiable_bookings_with_suggestion.Count)
            {
                messages.Add("Simulation failed!");
            }
            else
            {
                for(var index = 0; index < result.total_workload.Count; index++)
                {
                    string message = "index:" + index.ToString() +
                                     " workload:" + result.total_workload[index] +
                                     " num_bookings:" + result.num_generated_bookings[index] +
                                     " num_unsatisfiable:" + result.num_unsatisfiable_bookings[index];
                    messages.Add(message);
                }
            }

            foreach (var message in messages)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                var array_segment = new ArraySegment<byte>(bytes);
                await web_socket.SendAsync(array_segment, WebSocketMessageType.Text, true, CancellationToken.None);
                Thread.Sleep(100);
            }

            await web_socket.SendAsync(new ArraySegment<byte>(null), WebSocketMessageType.Binary, false,
                CancellationToken.None);
        }

        /// <summary>
        /// Evaluate the just running simulation (Automatically redirect after simulation finished)
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SimulationEvaluation(int simulation_result_id)
        {
            /* TODO:
             * SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
             * SimulationResult result = result_dao.GetByID(simulation_result_id);
             */
            SimulationResult result = new SimulationResult();
            return View(result);
        }
        
        [HttpGet]
        public IActionResult Infrastructure()
        {
            // Create Daos for Infrastructure
            LocationDao location_dao = new LocationDaoImpl(cache);
            ChargingZoneDao chargingzone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnDao chargingcolumn_dao = new ChargingColumnDaoImpl(cache);
            // Create ViewModel and set required daos
            InfrastructureViewModel view_model = new InfrastructureViewModel();
            List<Location> location = location_dao.GetAll(0);
            List<ChargingZone> chargingzone = chargingzone_dao.GetAll(0);
            List<ChargingColumn> chargingcolumn = chargingcolumn_dao.GetAll(0);
            view_model.locations = location;
            view_model.charging_zones = chargingzone;
            view_model.charging_columns = chargingcolumn;
            return View(view_model);
        }
        
        [HttpGet]
        public IActionResult Users()
        {
            UserDao user_dao = new UserDaoImpl(cache);
            List<User> users = user_dao.GetAll();
            return View(users);
        }
        
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new User());
        }
        
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            /* TODO:
             * UserDao user_dao = new UserDaoImpl(cache);
             * user_dao.Create(user.name, user.email, user.password, user.role);
             */
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult CreateBooking()
        {
            /* TODO:
             * return View(new Booking());
             */
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult CreateBooking(Booking booking)
        {
            /* TODO:
             * BookingDao booking_dao = new BookingDaoImpl(cache);
             * user_dao.Create(user.name, user.email, user.password, user.role, 1);
             */
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult EditBooking(int booking_id)
        {
            /* TODO:
             * BookingDao booking_dao = new BookingDaoImpl(cache);
             * Booking booking = booking_dao.GetById(booking_id);
             * return View(booking);
             */
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult EditBooking(Booking booking)
        {
            /* TODO:
             * BookingDao booking_dao = new BookingDaoImpl(cache);
             * booking_dao.Delete(booking_id, 1);
             * booking_dao.Create(booking.start_state_of_charge, booking.target_state_of_charge, booking.start_time, booking.end, booking.vehicle, booking.user, 1);
             */
            return RedirectToAction("Index");
        }
        
        
        [HttpGet]
        public IActionResult Vehicles()
        {
            VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
            List<Vehicle> vehicles = vehicle_dao.GetAll();
            return View(vehicles);
        }
        
        [HttpGet]
        public IActionResult CreateVehicle()
        {
            return View(new Vehicle());
        }

        [HttpPost]
        public IActionResult CreateVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid && vehicle.connector_types != null)
            {
                VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
                vehicle_dao.Create(vehicle.model_name, vehicle.capacity, vehicle.connector_types);
                return RedirectToAction("Vehicles");
            }
            return RedirectToAction("CreateVehicle");
        }
        

        /// <summary>
        /// Converts the given files to Dataobjects and refreshes the view to display these importchanges
        /// </summary>
        /// <param name="files">Given files</param>
        /// <returns>Redirection to index</returns>
        [HttpPost]
        public IActionResult Import(List<IFormFile> files)
        {
            if(ModelState.IsValid)
            {
                foreach(var file in files)
                {
                    if(file.ContentType == "application/json" && file.Length <= max_allowed_filesize)
                    {
                        //TODO: Import given file and store it
                    }
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Exports the Simulation Data as .json file
        /// </summary>
        /// <returns>A File which is Downloaded</returns>
        public IActionResult Export()
        {
            //TODO: Implement correct ListObjects/Data to export
            /*
            LIST = (List<>)_cache.Get("");
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(//FIXME: DATAOBJECT));
            var output = new FileContentResult(bytes, "application/octet-stream");
            output.FileDownloadName = "AdminSimulationData.json";
            return output;
            */

            //Placeholder
            return null;

        }
    }
}