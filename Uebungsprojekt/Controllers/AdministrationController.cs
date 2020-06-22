using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.OccupancyPlans;
using Uebungsprojekt.Simulations;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Controllers
{
    public class AdministrationController : Controller
    {
        private OccupancyPlan occupancy_plan;
        private Simulation simulation;

        /// <summary>
        /// Constructor for AdministrationController
        /// </summary>
        public AdministrationController()
        {
            // occupancy_plan = OccupancyPlan.OccupancyPlan.GetOccupancyPlan(LocationDaoImpl.GetAllLocations());            
        }

        /// <summary>
        /// General administration overview
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Show View containing the form for simulation parameters and infrastructure
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public IActionResult Simulation()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public void Simululation()
        {
            // TODO: Extract information from form and create simulation
            List<Tuple<DayOfWeek, TimeSpan>> rush_hours = new List<Tuple<DayOfWeek, TimeSpan>>();
            rush_hours.Add(Tuple.Create(DayOfWeek.Monday, new TimeSpan(8, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Tuesday, new TimeSpan(8, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Wednesday, new TimeSpan(8, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Thursday, new TimeSpan(8, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Friday, new TimeSpan(8, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Monday, new TimeSpan(16, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Tuesday, new TimeSpan(16, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Wednesday, new TimeSpan(16, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Thursday, new TimeSpan(16, 0, 0)));
            rush_hours.Add(Tuple.Create(DayOfWeek.Friday, new TimeSpan(12, 30, 0)));

            List<Object> locations = new List<Object>(); // TODO: Change to Location type

            simulation = new Simulation(
                new SimulationConfig
                {
                    tick_minutes = 15,
                    rush_hours = rush_hours,
                    min = 5,
                    max = 10,
                    spread = 0.5,
                    weeks = 3,
                    vehicles = new List<object>()
                },
                locations); // FIXME: Set parameters for config
        }

        public IActionResult Infrastructure()
        {
            return RedirectToAction("Index");
        }
    }
}