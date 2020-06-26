using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{

    [Authorize(Roles = "Employee")]
    public class AdministrationController : Controller
    {
        private readonly int max_allowed_filesize = (1024 * 1024) * 1; // Last multiplicator = mb
        private IMemoryCache _cache; // TODO: Evaluation


        /// <summary>
        /// Constructor for AdministrationController
        /// </summary>
        public AdministrationController()
        {
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
        public IActionResult SimulationConfig()
        {
            // FIXME: Return View()
            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Passes both the simulation infrastructure and configuration to the simulation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SimulationInfrastructure(SimulationConfig config)
        {
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Start and visualize the actual simulation
        /// </summary>
        /// <param name="infrastructure"></param>
        [HttpPost]
        public IActionResult Simulation(SimulationInfrastructure infrastructure)
        {
            return RedirectToAction("Index");
        }

        public IActionResult Evaluate(SimulationResult result)
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