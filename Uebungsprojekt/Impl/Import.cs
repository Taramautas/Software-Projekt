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

        public static int BookingImport(BookingDaoImpl _cache, List<IFormFile> json_files)
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
                    int id = BookingDaoImpl.CreateNewDaoId();
                    foreach (Booking b in importedBookings)

                        _cache.Create(b, id);

                    return id;
                }
            }
            return -1;
        }

        public static void ChargingColumnImport(ChargingColumnDaoImpl _cache, List<IFormFile> json_files)
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
                    int id = ChargingColumnDaoImpl.CreateNewDaoId();
                    foreach (ChargingColumn b in importedChargingColumn)

                        _cache.Create(b, id);


                }
            }
        }

       

        public static void LocationImport(LocationDaoImpl _cache, List<IFormFile> json_files)
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
                    int id = LocationDaoImpl.CreateNewDaoId();
                    foreach (Location b in importedLocation)

                        _cache.Create(b, id);


                }
            }
        }

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
                    int id = SimulationConfigDaoImpl.CreateNewDaoId();
                    foreach (SimulationConfig b in importedSimulationConfig)

                        _cache.Create(b, id);


                }
            }
        }

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
                List<SimulationInfrastructure> importedSimulationInfrastructure= JsonConvert.DeserializeObject<List<SimulationInfrastructure>>(json, settings);
                // If success, add to cached booking list
                if (success)
                {
                    int id = SimulationInfrastructureDaoImpl.CreateNewDaoId();
                    foreach (SimulationInfrastructure b in importedSimulationInfrastructure)

                        _cache.Create(b, id);


                }
            }
        }

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
                    int id = SimulationResultDaoImpl.CreateNewDaoId();
                    foreach (SimulationResult b in importedSimulationResult)

                        _cache.Create(b, id);


                }
            }
        }

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

                        _cache.Create(b, id);


                }
            }
        }

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
                    int id = VehicleDaoImpl.CreateNewDaoId();
                    foreach (Vehicle b in importedVehicle)

                        _cache.Create(b, id);


                }
            }
        }


        public static void Import(){}


    }
            
        
    
}
