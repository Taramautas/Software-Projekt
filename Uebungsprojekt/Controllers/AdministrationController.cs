using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.OccupancyPlans;
using Uebungsprojekt.Simulations;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{


    public class AdministrationController : Controller
    {
<<<<<<< HEAD
        private OccupancyPlan occupancy_plan;
        private Simulation simulation;
=======

        private readonly int max_allowed_filesize = (1024 * 1024) * 1; // Last multiplicator = mb
        private Object occupancy_plan; // FIXME: Adjust type when class is defined
        private Simulation.Simulation simulation; // FIXME: Adjust type when class is defined
        private IMemoryCache _cache; // TODO: Evaluation

>>>>>>> 9e81de96a5100ff3222b2cdb67b06a378e51672e

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

        /// <summary>
        /// Converts the given files to Dataobjects and refreshes the view to display these importchanges
        /// </summary>
        /// <param name="files">Given files</param>
        /// <returns>Redirection to index</returns>
        [HttpPost]
        public IActionResult Import(List<IFormFile> files)
        {
            if(ModelState.IsValid)
            {
                foreach(var file in files)
                {
                    if(file.ContentType == "application/json" && file.Length <= max_allowed_filesize)
                    {
                        //TODO: Import given file and store it
                    }
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Exports the Simulation Data as .json file
        /// </summary>
        /// <returns>A File which is Downloaded</returns>
        public IActionResult Export()
        {
            //TODO: Implement correct ListObjects/Data to export
            /*
            LIST = (List<>)_cache.Get("");
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(//FIXME: DATAOBJECT));
            var output = new FileContentResult(bytes, "application/octet-stream");
            output.FileDownloadName = "AdminSimulationData.json";
            return output;
            */

            //Placeholder
            return null;

        }
    }
}