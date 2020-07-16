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

namespace Uebungsprojekt.Impl
{
    public class Import
    {
        /// <summary>
        /// Imports Booking list from json file into specified dao id
        /// </summary>
        /// <param name="_cache">BookingDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        /// <param name="DaoId">Id of BookingDao</param>
        public static void BookingImport(BookingDaoImpl _cache, List<IFormFile> json_files, int DaoId)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<Booking> importedBookings = JsonConvert.DeserializeObject<List<Booking>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (Booking b in importedBookings)
                    {
                        _cache.Create(b.start_state_of_charge, b.target_state_of_charge, b.start_time, b.end_time, b.vehicle, b.user, b.location, DaoId);
                    }
                }
            }
        }

        /// <summary>
        /// Imports ChargingColumnImport list from json file into specified dao id
        /// </summary>
        /// <param name="_cache">ChargingColumnDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        /// <param name="DaoId">Id of ChargingColumn</param>
        public static void ChargingColumnImport(ChargingColumnDaoImpl _cache, List<IFormFile> json_files, int DaoId)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<ChargingColumn> importedChargingColumn = JsonConvert.DeserializeObject<List<ChargingColumn>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (ChargingColumn b in importedChargingColumn)
                    {
                        _cache.Create(b.charging_column_type_id, b.emergency_reserve, b.charging_zone, DaoId);
                    }
                }
            }
        }


        /// <summary>
        /// Imports LocationImport list from json file into specified dao id
        /// </summary>
        /// <param name="_cache">LocationDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        /// <param name="DaoId">Id of LocationDao</param>
        public static void LocationImport(LocationDaoImpl _cache, List<IFormFile> json_files, int DaoId)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<Location> importedLocation = JsonConvert.DeserializeObject<List<Location>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (Location b in importedLocation)
                    {
                        _cache.Create(b.city, b.post_code, b.address, DaoId);
                    }
                }
            }
        }

        /// <summary>
        /// Imports SimulationConfigImport list from json file into dao
        /// </summary>
        /// <param name="_cache">SimulationConfigDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        public static void SimulationConfigImport(SimulationConfigDaoImpl _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<SimulationConfig> importedSimulationConfig = JsonConvert.DeserializeObject<List<SimulationConfig>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (SimulationConfig b in importedSimulationConfig)
                    {
                        _cache.Create(b.tick_minutes, b.rush_hours, b.min, b.max, b.spread, b.weeks, b.vehicles);
                    }
                }
            }
        }

        /// <summary>
        /// Imports SimulationInfrastructureImport list from json file into dao
        /// </summary>
        /// <param name="_cache">SimulationInfrastructureDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        public static void SimulationInfrastructureImport(SimulationInfrastructureDaoImpl _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<SimulationInfrastructure> importedSimulationInfrastructure = JsonConvert.DeserializeObject<List<SimulationInfrastructure>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (SimulationInfrastructure b in importedSimulationInfrastructure)
                    {
                        _cache.Create(b.location_dao_id, b.charging_zone_dao_id, b.charging_column_dao_id);
                    }

                }
            }
        }

        /// <summary>
        /// Imports SimulationResultImport list from json file into dao
        /// </summary>
        /// <param name="_cache">SimulationResultDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        public static void SimulationResultImport(SimulationResultDaoImpl _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<SimulationResult> importedSimulationResult = JsonConvert.DeserializeObject<List<SimulationResult>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (SimulationResult b in importedSimulationResult)
                    {
                        _cache.Create(b.config, b.infrastructure, b.total_workload, b.num_generated_bookings, b.num_unsatisfiable_bookings, b.done);
                    }
                }
            }
        }

        /// <summary>
        /// Imports UserImport list from json file into dao
        /// </summary>
        /// <param name="_cache">UserDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        public static void UserImport(UserDaoImpl _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<User> importedUser = JsonConvert.DeserializeObject<List<User>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (User b in importedUser)
                    {
                        _cache.Create(b.name, b.email, b.password, b.role);
                    }
                }
            }
        }

        /// <summary>
        /// Imports VehicleImport list from json file into dao
        /// </summary>
        /// <param name="_cache">VehicleDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        public static void VehicleImport(VehicleDaoImpl _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<Vehicle> importedVehicle = JsonConvert.DeserializeObject<List<Vehicle>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (Vehicle b in importedVehicle)
                    {
                        _cache.Create(b.model_name, b.capacity, b.connector_types);
                    }
                }
            }
        }

        /// <summary>
        /// Imports ChargingColumnTypeImport list from json file into dao
        /// </summary>
        /// <param name="_cache">ChargingColumnTypeDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        public static void ChargingColumnTypeImport(ChargingColumnTypeDaoImpl _cache, List<IFormFile> json_files)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<ChargingColumnType> importedChargingColumnType = JsonConvert.DeserializeObject<List<ChargingColumnType>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (ChargingColumnType b in importedChargingColumnType)
                    {
                        _cache.Create(b.model_name, b.manufacturer_name, b.max_parallel_charging, b.connectors, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Imports ChargingZoneImport list from json file into specified dao id
        /// </summary>
        /// <param name="_cache">ChargingZoneDaoImpl cache</param>
        /// <param name="json_files">File with data thats to be imported</param>
        /// <param name="DaoId">Id of ChargingZoneDao</param>
        public static void ChargingZoneImport(ChargingZoneDaoImpl _cache, List<IFormFile> json_files, int DaoId)
        {
            // Server side validation: Check the file for .json extension and for max. size 1MB
            if (json_files[0].FileName.EndsWith(".json") && json_files[0].Length < 1000000)
            {
                // Deserialize list of bookings
                StreamReader reader = new StreamReader(json_files[0].OpenReadStream());
                string json = reader.ReadToEnd();
                bool success = true;
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                    MissingMemberHandling = MissingMemberHandling.Error
                };
                List<ChargingZone> importedChargingZone = JsonConvert.DeserializeObject<List<ChargingZone>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    foreach (ChargingZone b in importedChargingZone)
                    {
                        _cache.Create(b.name, b.overall_performance, b.location, DaoId);
                    }
                }
            }
        }
    }
}
