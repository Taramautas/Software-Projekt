using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Infrastructure()
        {
            return View();
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult SimulationVehicle()
        {
            return View();
        }

        public IActionResult SimulationTracking()
        {
            return View();
        }

        public IActionResult SimulationEvaluation()
        {
            return View();
        }

        public IActionResult Planer()
        {
            return View();
        }

        public IActionResult WIP()
        {
            return View();
        }

        public IActionResult SimulationInfrastructure()
        {
            return View();
        }

        public IActionResult SimulationSettings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
