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
        List<ConnectorTypeEvaluationViewModel> evalList;
        List<Booking> bookings = new List<Booking>();
        private IMemoryCache _cache;


        /// <summary>
        /// Constructor of controller. Staticly initialize memory cache
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for intializing the memory cache</param>
        public ConnectorTypeEvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

        }


        /// <summary>
        /// Updates the list containing percentages and returns View
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            updateEvaluationList();
            return View(evalList);
        }



        private void updateEvaluationList()
        {
            evalList = new List<ConnectorTypeEvaluationViewModel>();
            foreach(var type in Enum.GetValues(typeof(Booking.ConnectorType)).Cast<Booking.ConnectorType>())
            {
                evalList.Add(new ConnectorTypeEvaluationViewModel
                {
                    Con_type = type,
                    Percentage = 0.0
                });
            }
            bookings = (List<Booking>)_cache.Get("CreateBooking");
            
            if (bookings == null) return;
            if (bookings.Count == 0) return;

            foreach (var booking in bookings)
            {
                foreach(var eval in evalList)
                {
                    if (booking.Con_type.Equals(eval.Con_type)) eval.Percentage += 1;
                }
            }

            foreach(var eval in evalList)
            {
                eval.Percentage = Math.Round((eval.Percentage / bookings.Count) * 100);
            }
        }
    }
}
