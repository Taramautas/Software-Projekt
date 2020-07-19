using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Simulations;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc;

namespace Uebungsprojekt.Impl
{
    public class Import
    {
        public static bool ImportEverything(IMemoryCache _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Split Json file into all model lists
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                string[] lines = json.Split("\nNEXTSTRING\n");
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                // Initialize Daos
                ChargingColumnTypeDao chargingColumnTypeDao = new ChargingColumnTypeDaoImpl(_cache);
                List<ChargingColumnType> chargingColumnTypeList = chargingColumnTypeDao.GetAll();
                LocationDao locationDao = new LocationDaoImpl(_cache);
                List<Location> locationList = locationDao.GetAll(0);
                UserDao userDao = new UserDaoImpl(_cache);
                List<User> userList = userDao.GetAll();
                VehicleDao vehicleDao = new VehicleDaoImpl(_cache);
                List<Vehicle> vehicleList = vehicleDao.GetAll();
                ChargingZoneDao chargingZoneDao = new ChargingZoneDaoImpl(_cache);
                List<ChargingZone> chargingZoneList = chargingZoneDao.GetAll(0);
                ChargingColumnDao chargingColumnDao = new ChargingColumnDaoImpl(_cache);
                List<ChargingColumn> chargingColumnList = chargingColumnDao.GetAll(0);
                BookingDao bookingDao = new BookingDaoImpl(_cache);
                List<Booking> bookingList = bookingDao.GetAll(0);

                // Delete all current live data
                bookingList.Clear();
                chargingColumnList.Clear();
                chargingZoneList.Clear();
                vehicleList.Clear();
                userList.Clear();
                locationList.Clear();
                chargingColumnTypeList.Clear();
                

                // ids of imported objects
                // item1 = old id ; item2 = new id
                List<Tuple<int, int>> chargingColumnTypeIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> locationIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> userIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> vehicleIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> chargingZoneIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> chargingColumnIds = new List<Tuple<int, int>>();

                try
                {
                    // Deserialize ChargingColumnType

                    List<ChargingColumnType> importedChargingColumnType =
                        JsonConvert.DeserializeObject<List<ChargingColumnType>>(lines[0], settings);

                    // If success, add to cached booking list
                    if (success)
                    {
                        foreach (ChargingColumnType b in importedChargingColumnType)
                        {
                            chargingColumnTypeIds.Add(new Tuple<int, int>(b.id,
                                chargingColumnTypeDao.Create(b.model_name, b.manufacturer_name, b.max_parallel_charging,
                                    b.connectors)));
                        }
                    }

                    // Deserialize Location
                    List<Location> importedLocation = JsonConvert.DeserializeObject<List<Location>>(lines[1], settings);
                    // If success, add to cached chargingColumnType list
                    if (success)
                    {
                        foreach (Location b in importedLocation)
                        {
                            locationIds.Add(new Tuple<int, int>(b.id,
                                locationDao.Create(b.city, b.post_code, b.address, 0)));
                        }
                    }

                    // Deserialize User
                    List<User> importedUser = JsonConvert.DeserializeObject<List<User>>(lines[2], settings);
                    // If success, add to cached user list
                    if (success)
                    {
                        foreach (User b in importedUser)
                        {
                            userIds.Add(new Tuple<int, int>(b.id, userDao.Create(b.name, b.email, b.password, b.role)));
                        }
                    }

                    // Deserialize Vehicle
                    List<Vehicle> importedVehicle = JsonConvert.DeserializeObject<List<Vehicle>>(lines[3], settings);
                    // If success, add to cached vehicle list
                    if (success)
                    {
                        foreach (Vehicle b in importedVehicle)
                        {
                            int user_id = userIds.Find(x => x.Item1 == b.user.id).Item2;
                            vehicleIds.Add(new Tuple<int, int>(b.id,
                                vehicleDao.Create(b.model_name, b.capacity, b.connector_types,
                                    userDao.GetById(user_id))));
                        }
                    }

                    // Deserialize ChargingZone
                    List<ChargingZone> importedChargingZone =
                        JsonConvert.DeserializeObject<List<ChargingZone>>(lines[4], settings);
                    // If success, add to cached chargingZone list
                    if (success)
                    {
                        foreach (ChargingZone b in importedChargingZone)
                        {
                            int loc_id = locationIds.Find(x => x.Item1 == b.location.id).Item2;
                            chargingZoneIds.Add(new Tuple<int, int>(b.id,
                                chargingZoneDao.Create(b.name, b.overall_performance, locationDao.GetById(loc_id, 0),
                                    0)));
                        }
                    }

                    // Deserialize ChargingColumn
                    List<ChargingColumn> importedChargingColumn =
                        JsonConvert.DeserializeObject<List<ChargingColumn>>(lines[5], settings);
                    // If success, add to cached chargingColumn list
                    if (success)
                    {
                        foreach (ChargingColumn b in importedChargingColumn)
                        {
                            int cz_id = chargingZoneIds.Find(x => x.Item1 == b.charging_zone.id).Item2;
                            int cct_id = chargingColumnTypeIds.Find(x => x.Item1 == b.charging_column_type_id.id).Item2;
                            chargingColumnIds.Add(new Tuple<int, int>(b.id,
                                chargingColumnDao.Create(chargingColumnTypeDao.GetById(cct_id),
                                    chargingZoneDao.GetById(cz_id, 0), b.list, 0)));
                        }
                    }

                    // Deserialize Booking
                    List<Booking> importedBookings = JsonConvert.DeserializeObject<List<Booking>>(lines[6], settings);
                    // If success, add to cached booking list
                    if (success)
                    {
                        foreach (Booking b in importedBookings)
                        {
                            int veh_id = vehicleIds.Find(x => x.Item1 == b.vehicle.id).Item2;
                            int user_id = userIds.Find(x => x.Item1 == b.user.id).Item2;
                            int location_id = locationIds.Find(x => x.Item1 == b.location.id).Item2;
                            bookingDao.Create(b.start_state_of_charge, b.target_state_of_charge, b.start_time,
                                b.end_time, vehicleDao.GetById(veh_id), userDao.GetById(user_id),
                                locationDao.GetById(location_id, 0), 0);
                        }
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Deserializes json file and loads everything into daos and returns the id of the SimulationResult
        /// </summary>
        /// <param name="_cache"></param>
        /// <param name="json_files"></param>
        /// <returns>Id of SimulationResult on success and -1 on failure</returns>
        public static int ImportSimulationResult(IMemoryCache _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Split Json file into all model lists
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                string[] lines = json.Split("\nNEXTSTRING\n");
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                // Initialize Daos
                ChargingColumnTypeDao chargingColumnTypeDao = new ChargingColumnTypeDaoImpl(_cache);
                chargingColumnTypeDao.GetAll();
                LocationDao locationDao = new LocationDaoImpl(_cache);
                int locationDaoId = LocationDaoImpl.CreateNewDaoId();
                locationDao.GetAll(locationDaoId);
                ChargingZoneDao chargingZoneDao = new ChargingZoneDaoImpl(_cache);
                int chargingZoneDaoId = ChargingZoneDaoImpl.CreateNewDaoId();
                chargingZoneDao.GetAll(chargingZoneDaoId);
                ChargingColumnDao chargingColumnDao = new ChargingColumnDaoImpl(_cache);
                int chargingColumnDaoId = ChargingColumnDaoImpl.CreateNewDaoId();
                chargingColumnDao.GetAll(chargingColumnDaoId);
                SimulationInfrastructureDao simulationInfrastructureDao = new SimulationInfrastructureDaoImpl(_cache);
                simulationInfrastructureDao.GetAll();
                SimulationConfigDao simulationConfigDao = new SimulationConfigDaoImpl(_cache);
                simulationConfigDao.GetAll();
                SimulationResultDao simulationResultDao = new SimulationResultDaoImpl(_cache);
                simulationResultDao.GetAll();

                // ids of imported objects
                // item1 = old id ; item2 = new id
                List<Tuple<int, int>> chargingColumnTypeIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> locationIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> chargingZoneIds = new List<Tuple<int, int>>();
                List<Tuple<int, int>> chargingColumnIds = new List<Tuple<int, int>>();
                int simulationInfrastructureId;
                int simulationConfigId;

                // Deserialize ChargingColumnType
                List<ChargingColumnType> importedChargingColumnType = JsonConvert.DeserializeObject<List<ChargingColumnType>>(lines[0], settings);
                // If success, add to cached booking list
                
                if (success)
                {
                    foreach (ChargingColumnType b in importedChargingColumnType)
                    {
                        ChargingColumnType cct1 = null;
                        if ((cct1 = chargingColumnTypeDao.GetAll().Find(x => x.Equals(b))) != null)
                        {
                            chargingColumnTypeIds.Add(new Tuple<int, int>(b.id, cct1.id));
                        }
                        else
                        {
                            chargingColumnTypeIds.Add(new Tuple<int, int>(b.id, chargingColumnTypeDao.Create(b.model_name, b.manufacturer_name, b.max_parallel_charging, b.connectors)));
                        }
                    }
                }
                // Deserialize Location
                List<Location> importedLocation = JsonConvert.DeserializeObject<List<Location>>(lines[1], settings);
                // If success, add to cached chargingColumnType list
                if (success)
                {
                    foreach (Location b in importedLocation)
                    {
                        locationIds.Add(new Tuple<int, int>(b.id, locationDao.Create(b.city, b.post_code, b.address, locationDaoId)));
                    }
                }
                
                // Deserialize ChargingZone
                List<ChargingZone> importedChargingZone = JsonConvert.DeserializeObject<List<ChargingZone>>(lines[2], settings);
                // If success, add to cached chargingZone list
                if (success)
                {
                    foreach (ChargingZone b in importedChargingZone)
                    {
                        int loc_id = locationIds.Find(x => x.Item1 == b.location.id).Item2;
                        chargingZoneIds.Add(new Tuple<int, int>(b.id, chargingZoneDao.Create(b.name, b.overall_performance, locationDao.GetById(loc_id, locationDaoId), chargingZoneDaoId)));
                    }
                }

                // Deserialize ChargingColumn
                List<ChargingColumn> importedChargingColumn = JsonConvert.DeserializeObject<List<ChargingColumn>>(lines[3], settings);
                // If success, add to cached chargingColumn list
                if (success)
                {
                    foreach (ChargingColumn b in importedChargingColumn)
                    {
                        int cz_id = chargingZoneIds.Find(x => x.Item1 == b.charging_zone.id).Item2;
                        int cct_id = chargingColumnTypeIds.Find(x => x.Item1 == b.charging_column_type_id.id).Item2;
                        chargingColumnIds.Add(new Tuple<int, int>(b.id, chargingColumnDao.Create(chargingColumnTypeDao.GetById(cct_id), chargingZoneDao.GetById(cz_id, chargingZoneDaoId), b.list, chargingColumnDaoId)));
                    }
                }

                // Deserialize SimulationResult
                SimulationResult importedSimulationResult = JsonConvert.DeserializeObject<SimulationResult>(lines[4], settings);
                // If success, add to cached booking list
                if (success)
                {
                    // Add SimulationConfig
                    SimulationConfig simulationConfig = importedSimulationResult.config;
                    simulationConfigId = simulationConfigDao.Create(simulationConfig.tick_minutes, simulationConfig.rush_hours, simulationConfig.min, simulationConfig.max, simulationConfig.spread, simulationConfig.weeks, simulationConfig.vehicles);

                    // Add SimulationInfrastructure
                    SimulationInfrastructure simulationInfrastructure = importedSimulationResult.infrastructure;
                    simulationInfrastructureId = simulationInfrastructureDao.Create(locationDaoId, chargingZoneDaoId, chargingColumnDaoId);

                    // Add SimulationResult
                    int simulationResultId = simulationResultDao.Create(simulationConfigDao.GetById(simulationConfigId), simulationInfrastructureDao.GetById(simulationInfrastructureId), importedSimulationResult.total_workload, importedSimulationResult.num_generated_bookings, importedSimulationResult.num_unsatisfiable_bookings, importedSimulationResult.done);

                    return simulationResultId;
                }
                return -1;
            }
            return -1;
        }
    }
}
