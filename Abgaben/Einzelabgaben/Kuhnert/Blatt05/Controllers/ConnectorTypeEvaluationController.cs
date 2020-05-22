using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Uebungsprojekt.Controllers
{
    public class ConnectorTypeEvaluationController : Controller
    {
        List<ConnectorTypeEvaluationViewModel> evaluationList;
        List<Booking> bookings = new List<Booking>();
        private IMemoryCache _cache;


        /// <summary>
        /// Controller Constructer which using memoryCache
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for intializing the memory cache</param>
        public ConnectorTypeEvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

        }


        /// <summary>
        /// Create the View of the Evaluation List
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            updateEvaluationList();
            return View(evaluationList);
        }

        /// <summary>
        /// Updating the Evaluation List of Connector Types
        /// </summary>

        private void updateEvaluationList()
        {
            evaluationList = new List<ConnectorTypeEvaluationViewModel>();
            foreach (var type in Enum.GetValues(typeof(Booking.ConnectorType)).Cast<Booking.ConnectorType>())
            {
                evaluationList.Add(new ConnectorTypeEvaluationViewModel
                {
                    connectorType = type,
                    Percentage = 0.0
                });
            }
            bookings = (List<Booking>)_cache.Get("CreateBooking");

            if (bookings == null) return;
            if (bookings.Count == 0) return;

            foreach (var booking in bookings)
            {
                foreach (var eval in evaluationList)
                {
                    if (booking.connectorType.Equals(eval.connectorType)) eval.Percentage += 1;
                }
            }

            foreach (var eval in evaluationList)
            {
                eval.Percentage = Math.Round((eval.Percentage / bookings.Count) * 100);
            }
        }
    }
}
