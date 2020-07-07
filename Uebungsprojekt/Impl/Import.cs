using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Impl
{
    public class Import
    {

        public static void BookingImport(IMemoryCache _cache ,List<IFormFile> json_files)
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

                    if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
                    {
                        createdBookings.AddRange(importedBookings);
                    }
                    else
                    {
                        _cache.Set("CreateBooking", importedBookings);
                    }
                }
            }
        }
    }
}
