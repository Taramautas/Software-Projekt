using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;

namespace Uebungsprojekt.Controllers
{
    /*
    public class ConnectorTypeEvaluationController : Controller
    {
        /// <summary> Memory Cache </summary>
        private IMemoryCache _cache;
        /// <summary> List for passing to the view </summary>
        private List<ConnectorTypeEvaluationViewModel> _evaluation;

        /// <summary>
        /// Constructor of controller.
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for initializing the memory cache</param>
        public ConnectorTypeEvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;          
        }

        /// <summary>
        /// Retuns a list, displaying the proportion of each connector type in respect of the total amount of bookings
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            // Dict for counting the proportions of the different connector types
            Dictionary<Booking.ConnectorTypeEnum, int> proportions = new Dictionary<Booking.ConnectorTypeEnum, int>();
            List<Booking> bookings = new List<Booking>();
            _evaluation = new List<ConnectorTypeEvaluationViewModel>();

            foreach (Booking.ConnectorTypeEnum connectorType in Enum.GetValues(typeof(Booking.ConnectorTypeEnum)))
            {
                proportions[connectorType] = 0;
            }

            // Count occurrences of connector types in all bookings
            if (_cache.TryGetValue("CreateBooking", out List<Booking> cache_bookings))
            {
                bookings = cache_bookings;
                foreach (Booking b in bookings)
                {
                    proportions[b.ConnectorType] += 1;
                }
            }

            // Calculate the percentages and add ConnectorTypeEvaluation objects to view accordingly
            foreach (KeyValuePair<Booking.ConnectorTypeEnum, int> entry in proportions)
            {
                double proportion = entry.Value / (double)bookings.Count * 100;
                if (Double.IsNaN(proportion))
                {
                    proportion = 0.0;
                }
                _evaluation.Add(
                    new ConnectorTypeEvaluationViewModel(entry.Key, Double.IsNaN(proportion) ? 0.0 : proportion)
                );
            }
            return View(_evaluation);
        }

        /// <summary>
        /// Serializes and exports a list of all ConnectorTypeEvalution objects, formated as readable json
        /// </summary>
        /// <returns>List of ConnectorTypeEvalution objects as .json-file</returns>
        public IActionResult Export()
        {    
            // Serialize list ob ConnectorTypeEvaluation objects to readable json
            string json = JsonConvert.SerializeObject(_evaluation, Formatting.Indented);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var output = new FileContentResult(bytes, "application/octet-stream");
            string filename = "ConnectorTypeEvaluation_" + DateTime.Now.ToString(new CultureInfo("de-DE"))
                                                .Replace(":", "_")
                                                .Replace(".", "_") 
                                                .Replace(" ", "_")
                                            + ".json";
            output.FileDownloadName = filename;
            // Return json file for download
            return output;
        }
    }
    */
}
