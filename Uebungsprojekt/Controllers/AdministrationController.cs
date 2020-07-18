using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Diagnostics;

namespace Uebungsprojekt.Controllers
{

    // AdministrationController is only accessible for planners
    [Authorize(Roles = "Planner")]
    public class AdministrationController : Controller
    {
        private readonly int max_allowed_filesize = (1024 * 1024) * 1; // Last multiplicator = mb
        private IMemoryCache cache;
        private HttpContext http_context;
        private int user_id;


        /// <summary>
        /// Constructor for AdministrationController
        /// </summary>
        /// <param name="cache">IMemoryCache passed via Dependency Injection</param>
        public AdministrationController(UserManager user_manager, IMemoryCache cache, IHttpContextAccessor http_context_accessor)
        {
            this.cache = cache;
            this.http_context = http_context_accessor.HttpContext;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
        }

        /// <summary>
        /// Static description of all possible actions within the administration controller
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Converts the given files to Dataobjects and refreshes the view to display these importchanges
        /// </summary>
        /// <param name="files">Given files</param>
        /// <returns>Redirection to SimulationConfig</returns>
        [HttpPost]
        public IActionResult ImportSimulationResult(List<IFormFile> json_files)
        {
            SimulationResultDao simulationResultDao = new SimulationResultDaoImpl(cache);
            if (json_files.Count != 1)
            {
                return RedirectToAction("SimulationConfig");
            }
            SimulationResult simulationResult = simulationResultDao.GetById(Impl.Import.ImportSimulationResult(cache, json_files));
            // TODO: exceptions
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            };
            Response.Cookies.Append("SimulationConfig", simulationResult.config.id.ToString(), options);
            Response.Cookies.Append("SimulationInfrastructure", simulationResult.infrastructure.id.ToString(), options);
            Response.Cookies.Append("SimulationResult", simulationResult.id.ToString(), options);

            return RedirectToAction("SimulationConfig", simulationResult.config.id);
        }

        /// <summary>
        /// Exports all live Data as .json file
        /// </summary>
        /// <returns>A downloadable file</returns>
        public IActionResult ExportSimulationResult()
        {
            SimulationResult simulationResult = GetSimulationResultFromCookie();
            // TODO: exceptions
            return Impl.Export.ExportSimulationResult(cache, simulationResult.id);
        }

        /// <summary>
        /// Show View containing the form for simulation parameters
        /// </summary>
        [HttpGet]
        public IActionResult SimulationConfig()
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                config = new SimulationConfig();
            return View(config);
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
            
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            };
            Response.Cookies.Append("SimulationConfig", config_id.ToString(), options);
            
            return RedirectToAction("AddSimulationVehicle", config_id);
        }
        
        /// <summary>
        /// Return form for defining the simulation infrastructure
        /// </summary>
        /// <param name="simulation_config_id">int</param>
        [HttpGet]
        public IActionResult SimulationInfrastructure()
        {
            SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
            LocationDao location_dao = new LocationDaoImpl(cache);
            ChargingZoneDao charging_zone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnDao charging_column_dao = new ChargingColumnDaoImpl(cache);
            ChargingColumnTypeDao type_dao = new ChargingColumnTypeDaoImpl(cache);

            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config.rush_hours.Count == 0)
                return RedirectToAction("AddRushHours");
            if (infrastructure == null)
            {
                int infrastructure_id = infrastructure_dao.Create(
                    LocationDaoImpl.CreateNewDaoId(),
                    ChargingZoneDaoImpl.CreateNewDaoId(),
                    ChargingColumnDaoImpl.CreateNewDaoId()
                );
                infrastructure = infrastructure_dao.GetById(infrastructure_id);
                var options = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                };
                Response.Cookies.Append("SimulationInfrastructure", infrastructure_id.ToString(), options);
            }

            SimulationInfrastructureViewModel view_model = new SimulationInfrastructureViewModel
            {
                all_simulation_infrastructures = infrastructure_dao.GetAll(),
                simulation_infrastructure_id = infrastructure.id,
                charging_column_types = type_dao.GetAll(),
                locations = location_dao.GetAll(infrastructure.location_dao_id),
                charging_zones = charging_zone_dao.GetAll(infrastructure.charging_zone_dao_id),
                charging_columns = charging_column_dao.GetAll(infrastructure.charging_column_dao_id),
            };
            return View(view_model);
        }
        
        /// <summary>
        /// Redirect to SimulationConfig View (Start of the simulation configuration process) if not accessed via POST-Request
        /// </summary>
        [HttpGet]
        public IActionResult Simulation()
        {
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            SimulationConfig config = GetSimulationConfigFromCookie();
            LocationDao location_dao = new LocationDaoImpl(cache);
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);
            if (location_dao.GetAll(infrastructure.location_dao_id).Count == 0)
                return RedirectToAction("SimulationInfrastructure");
            if (charging_zone_dao.GetAll(infrastructure.charging_zone_dao_id).Count == 0)
                return RedirectToAction("SimulationInfrastructure");
            if (config == null)
                return RedirectToAction("SimulationConfig");
            if (infrastructure == null)
                return RedirectToAction("SimulationInfrastructure");
            
            SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
            int result_id = result_dao.Create(
                config, 
                infrastructure,
                new List<Dictionary<int, double>>(),
                new List<int>(),
                new List<int>(),
                false
                );
            SimulationResult result = result_dao.GetById(result_id);
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            };
            Response.Cookies.Append("SimulationResult", result_id.ToString(), options);

            Simulation simulation = new Simulation(config, infrastructure, result, cache);
            if (!simulation.Run())
            {
                Console.Out.WriteLine("Failure on simulation");
                return RedirectToPage("/Home/Error/");
            }
            
            SimulationViewModel view_model = new SimulationViewModel()
            {
                locations = location_dao.GetAll(infrastructure.location_dao_id),
                charging_zones = charging_zone_dao.GetAll(infrastructure.charging_zone_dao_id),
                result = simulation.simulation_result
            };
            return View(view_model);
        }


        
        /// <summary>
        /// Evaluate the simulation afterwards(Automatically redirect after simulation finished)
        /// </summary>
        /// <param name="simulation_result_id">int</param>
        [HttpGet]
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

        [HttpGet]
        public ActionResult DeleteLocation(int id)
        {
            LocationDaoImpl locationDao = new LocationDaoImpl(cache);
            locationDao.Delete(id, 0);
            return RedirectToAction("Infrastructure");
        }

        /// <summary>
        /// Display a complex table representing the current infrastructure
        /// </summary>
        [HttpGet]
        public IActionResult CreateSimulationLocation()
        {
            return View(new Location());
        }

        [HttpPost]
        public IActionResult CreateSimulationLocation(Location location)
        {
            if (ModelState.IsValid)
            {
                SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
                LocationDao location_dao = new LocationDaoImpl(cache);
                Console.WriteLine(location.city);
                Console.WriteLine(location.post_code);
                Console.WriteLine(location.address);
                Console.WriteLine(infrastructure.location_dao_id);
                location_dao.Create(location.city, location.post_code, location.address, infrastructure.location_dao_id);
            }
            return RedirectToAction("SimulationInfrastructure");
        }

        [HttpGet]
        public ActionResult DeleteSimulationLocation(int id)
        {
            LocationDaoImpl locationDao = new LocationDaoImpl(cache);
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure != null)
                locationDao.Delete(id, infrastructure.location_dao_id);
            return RedirectToAction("SimulationInfrastructure");
        }

        [HttpGet]
        public IActionResult CreateSimulationChargingZone()
        {
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure == null)
                return RedirectToAction("SimulationInfrastructure");
            
            LocationDao location_dao = new LocationDaoImpl(cache);
            CreateChargingZoneViewModel view_model = new CreateChargingZoneViewModel(
                location_dao.GetAll(infrastructure.location_dao_id), 
                new ChargingZone()
                );
            return View(view_model);
        }

        [HttpPost]
        public IActionResult CreateSimulationChargingZone(ChargingZone charge, int location_id)
        {
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure == null)
                return RedirectToAction("SimulationInfrastructure");
            LocationDao location_dao = new LocationDaoImpl(cache);
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);

            Console.WriteLine(charge.name + " performance: " + charge.overall_performance + "test");

            charging_zone_dao.Create(
                charge.name,
                charge.overall_performance, 
                location_dao.GetById(location_id, infrastructure.location_dao_id), 
                infrastructure.charging_zone_dao_id
                );

            return RedirectToAction("SimulationInfrastructure");
        }

        [HttpGet]
        public ActionResult DeleteSimulationChargingZone(int id)
        {
            ChargingZoneDaoImpl chargingZoneDao = new ChargingZoneDaoImpl(cache);
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure != null)
                chargingZoneDao.Delete(id, infrastructure.charging_zone_dao_id);
            return RedirectToAction("SimulationInfrastructure");
        }
        

        [HttpGet]
        public IActionResult AddSimulationChargingColumn()
        {
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure == null)
                return RedirectToAction("SimulationInfrastructure");
            
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnTypeDaoImpl charging_column_type_dao = new ChargingColumnTypeDaoImpl(cache);
            AddChargingColumnViewModel ccvm = new AddChargingColumnViewModel(
                charging_zone_dao.GetAll(infrastructure.charging_column_dao_id), 
                charging_column_type_dao.GetAll(), 
                new ChargingColumn()
                );
            return View(ccvm);
        }
        
        [HttpPost]
        public IActionResult AddSimulationChargingColumn(int charging_zone_id, int charging_column_type_id )
        {
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure == null)
                return RedirectToAction("SimulationInfrastructure");
            
            ChargingColumnTypeDaoImpl charging_column_type_dao = new ChargingColumnTypeDaoImpl(cache);
            ChargingColumnDaoImpl charging_column_dao = new ChargingColumnDaoImpl(cache);
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);
            
            charging_column_dao.Create(
                charging_column_type_dao.GetById(charging_column_type_id), 
                charging_zone_dao.GetById(charging_zone_id, infrastructure.charging_zone_dao_id), 
                null,
                infrastructure.charging_column_dao_id);
            return RedirectToAction("SimulationInfrastructure");
        }

        [HttpGet]
        public ActionResult DeleteSimulationChargingColumn(int id)
        {
            SimulationInfrastructure infrastructure = GetSimulationInfrastructureFromCookie();
            if (infrastructure == null)
                return RedirectToAction("SimulationInfrastructure");
            
            ChargingColumnDaoImpl chargingColumnTypeDao = new ChargingColumnDaoImpl(cache);
            chargingColumnTypeDao.Delete(id, infrastructure.charging_column_dao_id);
            return RedirectToAction("SimulationInfrastructure");
        }

        [HttpGet]
        public IActionResult AddSimulationVehicle()
        {
            VehicleDaoImpl vehicle_dao = new VehicleDaoImpl(cache);
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                return RedirectToAction("SimulationConfig");
            AddSimulationVehicleViewModel svvm = new AddSimulationVehicleViewModel(vehicle_dao.GetAll(), config);
            return View(svvm);
        }

        [HttpPost]
        public IActionResult AddSimulationVehicle(int vehicle_id, int count)
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                return RedirectToAction("SimulationConfig");
            
            VehicleDaoImpl vehicle_dao = new VehicleDaoImpl(cache);
            for(int i = 0; i < count; i++)
            {
                config.vehicles.Add(vehicle_dao.GetById(vehicle_id));
            }
            return RedirectToAction("AddSimulationVehicle");
        }

        [HttpGet]
        public ActionResult DeleteSimulationVehicle(int id)
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config != null)
            {
                VehicleDaoImpl vehicle_dao = new VehicleDaoImpl(cache);
                config.vehicles.Remove(vehicle_dao.GetById(id));
            }
            return RedirectToAction("AddSimulationVehicle");
        }

        [HttpGet]
        public ActionResult DeleteAllSimulationVehicles()
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                return RedirectToAction("SimulationConfig");
            
            config.vehicles.Clear();
            return RedirectToAction("AddSimulationVehicle");
        }

        [HttpGet]
        public IActionResult AddRushHours()
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                return RedirectToAction("SimulationConfig");
            if (config.vehicles.Count == 0)
                return RedirectToAction("AddSimulationVehicle");
            return View(config);
        }
        
        [HttpPost]
        public IActionResult AddRushHours(DayOfWeek weekday, TimeSpan timespan)
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                return RedirectToAction("SimulationConfig");
            
            config.rush_hours.Add(new Tuple<DayOfWeek, TimeSpan>(weekday, timespan));
            return View(config);
        }
        
        [HttpGet]
        public IActionResult DeleteRushHours(int index)
        {
            SimulationConfig config = GetSimulationConfigFromCookie();
            if (config == null)
                return RedirectToAction("SimulationConfig");
            
            config.rush_hours.RemoveAt(index);
            return RedirectToAction("AddRushHours");
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
            charging_zone.Create(charge.name,charge.overall_performance, location_dao.GetById(location_id, 0), 0);
            return RedirectToAction("Infrastructure");
        }

        [HttpGet]
        public ActionResult DeleteChargingZone(int id)
        {
            ChargingZoneDaoImpl chargingZoneDao = new ChargingZoneDaoImpl(cache);
            chargingZoneDao.Delete(id, 0);
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
            ChargingColumnTypeDaoImpl charging_column_type = new ChargingColumnTypeDaoImpl(cache);
            ChargingColumnDaoImpl charging_columnd_dao = new ChargingColumnDaoImpl(cache);
            ChargingZoneDaoImpl charging_zone_dao = new ChargingZoneDaoImpl(cache);
            charging_columnd_dao.Create(
                charging_column_type.GetById(charging_column_type_id), 
                charging_zone_dao.GetById(charging_zone_id, 0), 
                null,
                0
                );
            return RedirectToAction("Infrastructure");
        }

        [HttpGet]
        public ActionResult DeleteChargingColumn(int id)
        {
            ChargingColumnDaoImpl chargingColumnDao = new ChargingColumnDaoImpl(cache);
            chargingColumnDao.Delete(id, 0);
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
        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            BookingDaoImpl booking_dao = new BookingDaoImpl(cache);
            booking_dao.Delete(id, 0);
            return RedirectToAction("Bookings");
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
                UserDao user_dao = new UserDaoImpl(cache);
                vehicle_dao.Create(vehicle.model_name, vehicle.capacity, vehicle.connector_types, user_dao.GetById(user_id));
                return RedirectToAction("Vehicles");
            }
            return RedirectToAction("Vehicles");
        }

        [HttpGet]
        public ActionResult DeleteVehicle(int id)
        {
            VehicleDaoImpl vehicleDao = new VehicleDaoImpl(cache);
            vehicleDao.Delete(id);
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
                if(capacity1 > 0 && capacity1 <= 10000)
                    _connectors.Add(new Tuple<ConnectorType, int>(conn_1, capacity1));
            }

            if (connectortype2 != "None")
            {
                Enum.TryParse(connectortype2, out ConnectorType conn_2);
                if(capacity2 > 0 && capacity2 <= 10000)
                    _connectors.Add(new Tuple<ConnectorType, int>(conn_2, capacity2));
            }
            
            if (connectortype3 != "None")
            {
                Enum.TryParse(connectortype3, out ConnectorType conn_3);
                if(capacity3 > 0 && capacity3 <= 10000)
                    _connectors.Add(new Tuple<ConnectorType, int>(conn_3, capacity3));
            }
            
            if (connectortype4 != "None")
            {
                Enum.TryParse(connectortype4, out ConnectorType conn_4);
                if(capacity4 > 0 && capacity4 <= 10000)
                    _connectors.Add(new Tuple<ConnectorType, int>(conn_4, capacity4));
            }
            //Checks if any Connecters got added - if not -> redirect
            if (_connectors.Count == 0)
                return RedirectToAction("CreateChargingColumnType");
            
            cct_dao.Create(
                cct.model_name, 
                cct.manufacturer_name, 
                _connectors.Count, 
                _connectors);
            return RedirectToAction("ChargingColumnType");
        }

        [HttpGet]
        public ActionResult DeleteChargingColumnType(int id)
        {
            ChargingColumnTypeDaoImpl chargingColumnTypeDao = new ChargingColumnTypeDaoImpl(cache);
            chargingColumnTypeDao.Delete(id);
            return RedirectToAction("ChargingColumnType");
        }

        /// <summary>
        /// Converts the given files to Dataobjects and refreshes the view to display these importchanges
        /// </summary>
        /// <param name="files">Given files</param>
        /// <returns>Redirection to index</returns>
        [HttpPost]
        public IActionResult ImportEverything(List<IFormFile> json_files)
        {
            if (json_files.Count == 1)
            {
                Impl.Import.ImportEverything(cache, json_files);
            }
            // TODO: exceptions
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Exports all live Data as .json file
        /// </summary>
        /// <returns>A File which is Downloaded</returns>
        public IActionResult ExportEverything()
        {
            // TODO: exceptions
            return Impl.Export.ExportEverything(cache);
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

        private SimulationInfrastructure GetSimulationInfrastructureFromCookie()
        {
            SimulationInfrastructureDao infrastructure_dao = new SimulationInfrastructureDaoImpl(cache);
            Request.Cookies.TryGetValue("SimulationInfrastructure", out string infrastructure_string);
            if (string.IsNullOrEmpty(infrastructure_string))
                return null;
            SimulationInfrastructure infrastructure = infrastructure_dao.GetById(Int32.Parse(infrastructure_string));
            return infrastructure;
        }
        
        private SimulationConfig GetSimulationConfigFromCookie()
        {
            SimulationConfigDao config_dao = new SimulationConfigDaoImpl(cache);
            Request.Cookies.TryGetValue("SimulationConfig", out string config_string);
            if (string.IsNullOrEmpty(config_string))
                return null;
            SimulationConfig config = config_dao.GetById(Int32.Parse(config_string));
            return config;
        }
        
        private SimulationResult GetSimulationResultFromCookie()
        {
            SimulationResultDao result_dao = new SimulationResultDaoImpl(cache);
            Request.Cookies.TryGetValue("SimulationConfig", out string result_string);
            if (string.IsNullOrEmpty(result_string))
                return null;
            SimulationResult result = result_dao.GetById(Int32.Parse(result_string));
            return result;
        }
    }
}