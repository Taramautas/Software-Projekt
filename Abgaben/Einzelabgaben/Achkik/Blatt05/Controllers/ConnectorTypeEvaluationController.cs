using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;

namespace Uebungsprojekt.Controllers
{
    public class ConnectorTypeEvaluationController : Controller
    {
        private IMemoryCache _cache;

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
            // List for passing to the view
            List<ConnectorTypeEvaluationViewModel> connector_types = new List<ConnectorTypeEvaluationViewModel>();
            // Dict for counting the proportions of the different connector types
            Dictionary<Booking.ConnectorType, int> proportions = new Dictionary<Booking.ConnectorType, int>();
            // Counting the total amount of bookings
            int total_count = 0;
            
            // Count occurrences of connector types in all bookings
            if (_cache.TryGetValue("CreateBooking", out List<Booking> bookings))
            {
                foreach (Booking b in bookings)
                {
                    if (!proportions.ContainsKey(b.Connector_Type))
                    {
                        proportions[b.Connector_Type] = 1;
                    }
                    else
                    {
                        proportions[b.Connector_Type] += 1;
                    }
                    total_count += 1;
                }
                // Calculate the percentages and add ConnectorTypeEvaluation objects to view accordingly
                foreach (KeyValuePair<Booking.ConnectorType, int> entry in proportions)
                {
                    double proportion = entry.Value / (double)total_count * 100;
                    connector_types.Add(
                        new ConnectorTypeEvaluationViewModel(entry.Key, proportion)
                        );
                }
            }
            return View(connector_types);
        }
    }
}
