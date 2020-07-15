using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
