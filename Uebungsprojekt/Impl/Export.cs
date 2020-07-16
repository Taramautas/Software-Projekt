using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Impl
{
    public class Export
    {
        public static FileContentResult BookingExport(IMemoryCache _cache, List<Booking> bookings)
        {
                // Serialize booking list
                string json = JsonConvert.SerializeObject(bookings, Formatting.Indented);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                //Create downloadable file
                var output = new FileContentResult(bytes, "application/octet-stream");
                string filename = "Bookings_" + DateTime.Now.ToString(new CultureInfo("de-DE"))
                    .Replace(":", "_")
                    .Replace(".", "_")
                    .Replace(" ", "_")
                                              + ".json";
                output.FileDownloadName = filename;
                // Return .json file for download
                return output;
        }

        /// <summary>
        /// Exports the whole System (only live data)
        /// </summary>
        /// <param name="_cache">the cache which the Daos use</param>
        /// <returns></returns>
        public static FileContentResult ExportEverything(IMemoryCache _cache)
        {
            // Create Daos with cache
            ChargingColumnTypeDao chargingColumnTypeDao = new ChargingColumnTypeDaoImpl(_cache);
            LocationDao locationDao = new LocationDaoImpl(_cache);
            UserDao userDao = new UserDaoImpl(_cache);
            VehicleDao vehicleDao = new VehicleDaoImpl(_cache);
            ChargingZoneDao chargingZoneDao = new ChargingZoneDaoImpl(_cache);
            ChargingColumnDao chargingColumnDao = new ChargingColumnDaoImpl(_cache);
            BookingDao bookingDao = new BookingDaoImpl(_cache);

            // Get Lists from Daos
            List<ChargingColumnType> chargingColumnTypes = chargingColumnTypeDao.GetAll();
            List<Location> locations = locationDao.GetAll(0);
            List<User> users = userDao.GetAll();
            List<Vehicle> vehicles = vehicleDao.GetAll();
            List<ChargingZone> chargingZones = chargingZoneDao.GetAll(0);
            List<ChargingColumn> chargingColumns = chargingColumnDao.GetAll(0);
            List<Booking> bookings = bookingDao.GetAll(0);

            // Serialize lists
            string chargingColumnTypeString = JsonConvert.SerializeObject(chargingColumnTypes, Formatting.Indented);
            string locationString = JsonConvert.SerializeObject(locations, Formatting.Indented);
            string userString = JsonConvert.SerializeObject(users, Formatting.Indented);
            string vehicleString = JsonConvert.SerializeObject(vehicles, Formatting.Indented);
            string chargingZoneString = JsonConvert.SerializeObject(chargingZones, Formatting.Indented);
            string chargingColumnString = JsonConvert.SerializeObject(chargingColumns, Formatting.Indented);
            string bookingString = JsonConvert.SerializeObject(bookings, Formatting.Indented);

            string json = chargingColumnTypeString + "\nNEXTSTRING\n" + locationString + "\nNEXTSTRING\n" + userString + "\nNEXTSTRING\n" + vehicleString + "\nNEXTSTRING\n" + chargingZoneString + "\nNEXTSTRING\n" + chargingColumnString + "\nNEXTSTRING\n" + bookingString;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            //Create downloadable file
            var output = new FileContentResult(bytes, "application/octet-stream");
            string filename = "SystemExport_" + DateTime.Now.ToString(new CultureInfo("de-DE"))
                .Replace(":", "_")
                .Replace(".", "_")
                .Replace(" ", "_")
                                          + ".json";
            output.FileDownloadName = filename;
            // Return .json file for download
            return output;
        }
    }
}
