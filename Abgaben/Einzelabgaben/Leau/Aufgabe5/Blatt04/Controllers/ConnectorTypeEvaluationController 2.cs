using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;

namespace Uebungsprojekt.Controllers
{
    public class ConnectorTypeEvaluationController : Controller
    {
        private IMemoryCache _cache;

        public ConnectorTypeEvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public IActionResult Index()
        {
            List<ConnectorTypeEvaluationViewModel> connectorTypes = new List<ConnectorTypeEvaluationViewModel>();

            if (_cache.TryGetValue("CreateBooking", out List<Booking> created_bookings))
            {
                // percentage
            }
            return View(connectorTypes);
        }
    }
}
