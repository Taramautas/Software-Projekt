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
        public ConnectorTypeEvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            // Dict for counting the proportions of the different connector types
            Dictionary<Booking.ConnectorType, int> proportions = new Dictionary<Booking.ConnectorType, int>();
            List<Booking> bookings = new List<Booking>();
            _evaluation = new List<ConnectorTypeEvaluationViewModel>();

            foreach (Booking.ConnectorType connectorType in Enum.GetValues(typeof(Booking.ConnectorType)))
            {
                proportions[connectorType] = 0;
            }

            // Count occurrences of connector types in all bookings
            if (_cache.TryGetValue("CreateBooking", out List<Booking> cache_bookings))
            {
                bookings = cache_bookings;
                foreach (Booking b in bookings)
                {
                    proportions[b.Connector_Type] += 1;
                }
            }

            // Calculate the percentages and add ConnectorTypeEvaluation objects to view accordingly
            foreach (KeyValuePair<Booking.ConnectorType, int> entry in proportions)
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
        }
    }
}
