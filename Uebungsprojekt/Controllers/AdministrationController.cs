using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.OccupancyPlan;
using Uebungsprojekt.Simulation;

namespace Uebungsprojekt.Controllers
{
    public class AdministrationController : Controller
    {
        private Object occupancy_plan; // FIXME: Adjust type when class is defined
        private Object simulation; // FIXME: Adjust type when class is defined

        public AdministrationController()
        {
            occupancy_plan = OccupancyPlan.OccupancyPlan.GetOccupancyPlan();            
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Simulation()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public void Simululation()
        {
            // TODO: Extract information from form and create simulation
            // simulation = new Simulation(...);
        }

        public IActionResult Infrastructure()
        {
            return RedirectToAction("Index");
        }
    }
}