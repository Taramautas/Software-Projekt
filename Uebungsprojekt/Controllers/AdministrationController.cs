using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Simulations;
using Uebungsprojekt.ViewModel.Administration;

namespace Uebungsprojekt.Controllers
{

    // AdministrationController is only accessible for planners
    [Authorize(Roles = "Planner")]
    public class AdministrationController : Controller
    {
        private readonly int max_allowed_filesize = (1024 * 1024) * 1; // Last multiplicator = mb
        private IMemoryCache cache;


        /// <summary>
        /// Constructor for AdministrationController
        /// </summary>
        /// <param name="cache">IMemoryCache passed via Dependency Injection</param>
        public AdministrationController(IMemoryCache cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Static description of all possible actions within the administration controller
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Show View containing the form for simulation parameters
        /// </summary>
        [HttpGet]
        public IActionResult SimulationConfig()
        {
            return View(new SimulationConfig());
        }
        
        /// <summary>
        /// Save specified parameters as SimulationConfig object and redirect to form for SimulationInfrastructure
        /// </summary>
        /// <param name="config">SimulationConfig</param>
        [HttpPost]
        public IActionResult SimulationConfig(SimulationConfig config)
        {
            SimulationConfigDao config_dao = new SimulationConfigDaoImpl(cache);
            int config_id = config_dao.Create(config.tick_minutes, config.rush_hours, config.min, config.max, config.spread, config.weeks, config.vehicles);
            return RedirectToAction("SimulationInfrastructure", config_id);
        }
        
        /// <summary>
        /// Return form for defining the simulation infrastructure
        /// </summary>
        /// <param name="simulation_config_id">int</param>
        [HttpGet]
        public IActionResult SimulationInfrastructure(int simulation_config_id)
        {
             SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
             ChargingColumnTypeDao type_dao = new ChargingColumnTypeDaoImpl(cache);

             SimulationInfrastructureViewModel view_model = new SimulationInfrastructureViewModel()
             {
                 all_simulation_infrastructures = infrastructure_dao.GetAll(),
                 simulation_config_id = simulation_config_id,
                 charging_column_types = type_dao.GetAll(),
             };
             return View(view_model);
        }
        
        /// <summary>
        /// Save infrastructure as SimulationInfrastructure object and pass both the id of simulation and config to Simulation
        /// </summary>
        /// <param name="view_model">SimulationInfrastructureViewModel</param>
        [HttpPost]
        public IActionResult SimulationInfrastructure(SimulationInfrastructureViewModel view_model)
        {
            int location_dao_id = LocationDaoImpl.CreateNewDaoId();
            LocationDao location_dao = new LocationDaoImpl(cache);
            foreach (Location location in view_model.locations)
                location_dao.Create(location.city, location.post_code, location.address, location_dao_id);

            int charging_zone_dao_id = ChargingZoneDaoImpl.CreateNewDaoId();
            ChargingZoneDao charging_zone_dao = new ChargingZoneDaoImpl(cache);
            foreach (ChargingZone charging_zone in view_model.charging_zones)
                charging_zone_dao.Create(charging_zone.overall_performance, charging_zone.location, charging_zone_dao_id);

            int charging_column_dao_id = ChargingColumnDaoImpl.CreateNewDaoId();
            ChargingColumnDao charging_column_dao = new ChargingColumnDaoImpl(cache);
            foreach (ChargingColumn charging_column in view_model.charging_columns)
                charging_column_dao.Create(charging_column.charging_column_type_id,  charging_column.emergency_reserve, charging_column.charging_zone, charging_column_dao_id);
            
            SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
            int infrastructure_id = infrastructure_dao.Create(location_dao_id, charging_zone_dao_id, charging_column_dao_id);
            SimulationViewModel simulation_view_model = new SimulationViewModel()
            {
                simulation_config_id = view_model.simulation_config_id,
                simulation_infrastructure_id = infrastructure_id,
            };
            return RedirectToAction("Simulation", simulation_view_model);
        }
        
        /// <summary>
        /// Redirect to SimulationConfig View (Start of the simulation configuration process) if not accessed via POST-Request
        /// </summary>
        [HttpGet]
        public IActionResult Simulation()
        {
            return RedirectToAction("SimulationConfig");
        }


        /// <summary>
        /// Start and visualize the actual simulation
        /// </summary>
        /// <param name="view_model">SimulationViewModel</param>
        [HttpPost]
        public IActionResult Simulation(SimulationViewModel view_model)
        {
            SimulationConfigDao config_dao = new SimulationConfigDaoImpl(cache);
            SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);

            SimulationConfig config = config_dao.GetById(view_model.simulation_config_id);
            SimulationInfrastructure infrastructure =
                infrastructure_dao.GetById(view_model.simulation_infrastructure_id);
            SimulationResult result = new SimulationResult()
            {
                config = config,
                infrastructure = infrastructure,
            };
            
            
            Simulation simulation = new Simulation(config, infrastructure, result, cache);
            if (!simulation.Run())
            {
                Console.Out.WriteLine("Failure on simulation");
                return RedirectToPage("/Home/Error/");
            }
            return View(simulation.simulation_result);
        }

        /*
        [HttpPost]
        public async Task GetSimulationResults(int simulation_result_id)
        {
             SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
             SimulationResult result = result_dao.GetById(simulation_result_id);
                        
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
        */

        /// <summary>
        /// Evaluate the simulation afterwards(Automatically redirect after simulation finished)
        /// </summary>
        /// <param name="simulation_result_id">int</param>
        [HttpPost]
        public IActionResult SimulationEvaluation(int simulation_result_id)
        {
            SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
            SimulationResult result = result_dao.GetById(simulation_result_id);
            return View(result);
        }
        
        /// <summary>
        /// Display a complex table representing the current infrastructure
        /// </summary>
        [HttpGet]
        public IActionResult Infrastructure()
        {
            // Create Daos for Infrastructure
            LocationDao location_dao = new LocationDaoImpl(cache);
            ChargingZoneDao charging_zone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnDao charging_column_dao = new ChargingColumnDaoImpl(cache);

            InfrastructureViewModel view_model = new InfrastructureViewModel()
            {
                locations = location_dao.GetAll(0),
                charging_zones = charging_zone_dao.GetAll(0),
                charging_columns = charging_column_dao.GetAll(0),
            };
            return View(view_model);
        }

        /// <summary>
        /// Display a complex table representing the current infrastructure
        /// </summary>
        [HttpGet]
        public IActionResult CreateLocation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateLocation(Location location)
        {
            if (ModelState.IsValid)
            {
                LocationDao location_dao = new LocationDaoImpl(cache);
                location_dao.Create(location.city, location.post_code, location.address, 0);
                Console.WriteLine(location.city);
            }
            return RedirectToAction("Infrastructure");
        }

        /// <summary>
        /// Display a complex table representing the current infrastructure
        /// </summary>
        [HttpGet]
        public IActionResult CreateChargingZone()
        {
            LocationDao location_dao = new LocationDaoImpl(cache);
            var cczvm = new CreateChargingZoneViewModel(location_dao.GetAll(0), new ChargingZone());
            return View(cczvm);
        }

        [HttpPost]
        public IActionResult CreateChargingZone(ChargingZone charge, int location_id)
        {
            ChargingZoneDaoImpl charging_zone = new ChargingZoneDaoImpl(cache);
            LocationDaoImpl location_dao = new LocationDaoImpl(cache);
            charging_zone.Create(charge.overall_performance, location_dao.GetById(location_id, 0), 0);
            return RedirectToAction("Infrastructure");
        }

        /// <summary>
        /// Display a complex table representing the current infrastructure
        /// </summary>
        [HttpGet]
        public IActionResult AddChargingColumn()
        {
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnTypeDaoImpl charging_column_type_dao = new ChargingColumnTypeDaoImpl(cache);
            AddChargingColumnViewModel ccvm = new AddChargingColumnViewModel(charging_zone_dao.GetAll(0), charging_column_type_dao.GetAll(), new ChargingColumn());
            return View(ccvm);
        }

        [HttpPost]
        public IActionResult AddChargingColumn(int charging_zone_id, int charging_column_type_id )
        {
            //TODO: ERRORHANDLING!
            Console.WriteLine(charging_column_type_id);
            ChargingColumnTypeDaoImpl charging_column_type = new ChargingColumnTypeDaoImpl(cache);
            ChargingColumnDaoImpl charging_columnd_dao = new ChargingColumnDaoImpl(cache);
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);
            charging_columnd_dao.Create(charging_column_type.GetById(charging_column_type_id), false,
                charging_zone_dao.GetById(charging_zone_id, 0), 0);
            return RedirectToAction("Infrastructure");
        }


        /// <summary>
        /// Display a table of all bookings in system
        /// </summary>
        [HttpGet]
        public IActionResult Bookings()
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            return View(booking_dao.GetAll(0));
        }

        /// <summary>
        /// Show Create form for Booking
        /// </summary>
        [HttpGet]
        public IActionResult CreateBooking()
        {
            return View(new Booking());
        }
        
        /// <summary>
        /// Add booking to DAO if valid and return to Bookings
        /// </summary>
        /// <param name="booking">Booking</param>
        [HttpPost]
        public IActionResult CreateBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                BookingDao booking_dao = new BookingDaoImpl(cache);
                booking_dao.Create(
                    booking.start_state_of_charge, 
                    booking.target_state_of_charge, 
                    booking.start_time, 
                    booking.end_time, 
                    booking.vehicle, 
                    booking.user, 
                    booking.location, 
                    0);
                return RedirectToAction("Bookings");
            }
            return View(new Booking());
        }
        
        /// <summary>
        /// Show Edit form for bookings (same as Create, but already filled)
        /// </summary>
        /// <param name="booking_id">int</param>
        [HttpGet]
        public IActionResult EditBooking(int booking_id)
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            Booking booking = booking_dao.GetById(booking_id, 0);
            return View(booking);
        }
        
        /// <summary>
        /// Delete and recreate booking with new data if valid. Return to Bookings afterwards.
        /// </summary>
        /// <param name="booking">Booking</param>
        [HttpPost]
        public IActionResult EditBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                BookingDao booking_dao = new BookingDaoImpl(cache);
                booking_dao.Delete(booking.id, 0);
                booking_dao.Create(
                    booking.start_state_of_charge, 
                    booking.target_state_of_charge, 
                    booking.start_time, 
                    booking.end_time, 
                    booking.vehicle, 
                    booking.user, 
                    booking.location, 
                    0);
                return RedirectToAction("Bookings");
            }
            return View(booking);
        }
        
        /// <summary>
        /// Display a table of all vehicles in system
        /// </summary>
        [HttpGet]
        public IActionResult Vehicles()
        {
            VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
            return View(vehicle_dao.GetAll());
        }
        
        /// <summary>
        /// Show Create form for vehicles
        /// </summary>
        [HttpGet]
        public IActionResult CreateVehicle()
        {
            return View(new Vehicle());
        }

        /// <summary>
        /// Add vehicle to DAO if valid and return to Vehicles
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        [HttpPost]
        public IActionResult CreateVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid && vehicle.connector_types != null)
            {
                VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
                vehicle_dao.Create(vehicle.model_name, vehicle.capacity, vehicle.connector_types);
                return RedirectToAction("Vehicles");
            }
            return RedirectToAction("Vehicles");
        }

        [HttpGet]
        public IActionResult ChargingColumnType()
        {
            // Create Daos for Infrastructure
            ChargingColumnTypeDao charging_column_type_dao = new ChargingColumnTypeDaoImpl(cache);

            return View(charging_column_type_dao.GetAll());
        }

        [HttpGet]
        public IActionResult CreateChargingColumnType()
        {
            return View(new ChargingColumnType());
        }

        [HttpPost]
        public IActionResult CreateChargingColumnType(ChargingColumnType cct, String connectortype1, String connectortype2, String connectortype3, String connectortype4, int capacity1, int capacity2, int capacity3, int capacity4)
        {
            ChargingColumnTypeDaoImpl cct_dao = new ChargingColumnTypeDaoImpl(cache);
            List<Tuple<ConnectorType, int>> _connectors = new List<Tuple<ConnectorType, int>>();
            
            if (connectortype1 != "None")
            {
                Enum.TryParse(connectortype1, out ConnectorType conn_1);
                _connectors.Add(new Tuple<ConnectorType, int>(conn_1, capacity1));
            }

            if (connectortype2 != "None")
            {
                Enum.TryParse(connectortype2, out ConnectorType conn_2);
                _connectors.Add(new Tuple<ConnectorType, int>(conn_2, capacity2));
            }
            
            if (connectortype3 != "None")
            {
                Enum.TryParse(connectortype3, out ConnectorType conn_3);
                _connectors.Add(new Tuple<ConnectorType, int>(conn_3, capacity3));
            }
            
            if (connectortype4 != "None")
            {
                Enum.TryParse(connectortype4, out ConnectorType conn_4);
                _connectors.Add(new Tuple<ConnectorType, int>(conn_4, capacity4));
            }
            
            
            cct_dao.Create(cct.model_name, cct.manufacturer_name, cct.max_parallel_charging, _connectors);
            return RedirectToAction("ChargingColumnType");
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