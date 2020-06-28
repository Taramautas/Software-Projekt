using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Simulations;
using Uebungsprojekt.ViewModel.Administration;

namespace Uebungsprojekt.Controllers
{

    [Authorize(Roles = "Employee")]
    public class AdministrationController : Controller
    {
        private readonly UserManager user_manager;
        private string user_id;
        
        private readonly int max_allowed_filesize = (1024 * 1024) * 1; // Last multiplicator = mb
        private IMemoryCache _cache; // TODO: Evaluation


        /// <summary>
        /// Constructor for AdministrationController
        /// </summary>
        public AdministrationController(UserManager user_manager, IHttpContextAccessor http_context_accessor)
        {
            this.user_manager = user_manager;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
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
            SimulationConfigViewModel view_model = new SimulationConfigViewModel();
            // view_model.all_simulation_configs = SimulationConfigDao.GetDao().GetAllSimulationConfigs();
            return View(view_model);
        }

        /// <summary>
        /// Passes both the simulation infrastructure and configuration to the simulation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SimulationInfrastructure(SimulationInfrastructureViewModel view_model)
        {
            // TODO: view_model.all_simulation_infrastructures = SimulationInfrastructureDao.GetAllSimulationInfrastructures();
            return View(view_model);
        }

        /// <summary>
        /// Start and visualize the actual simulation
        /// </summary>
        /// <param name="view_model"></param>
        [HttpPost]
        public IActionResult Simulation(SimulationViewModel view_model)
        {
            Simulation simulation = new Simulation(view_model.simulation_config, view_model.simulation_infrastructure);
            if (!simulation.RunSimulation())
            {
                return RedirectToPage("/Home/Error/");
            }
            
            return View(simulation.simulation_result.id);
        }

        [HttpPost]
        public async Task GetSimulationResults(Simulation simulation)
        {
            var context = ControllerContext.HttpContext;
            var is_socket_request = context.WebSockets.IsWebSocketRequest;

            if (is_socket_request)
            {
                WebSocket web_socket = await context.WebSockets.AcceptWebSocketAsync();
                if (!simulation.RunSimulation())
                    await SendSimulationUpdates(context, web_socket, null);
                else
                    await SendSimulationUpdates(context, web_socket, simulation.simulation_result);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task SendSimulationUpdates(HttpContext context, WebSocket web_socket, SimulationResult result)
        {
            List<string> messages = new List<string>();
            if (result == null || 
                result.num_generated_bookings.Count != result.total_workload.Count || 
                result.total_workload.Count != result.unsatisfiable_bookings_with_suggestion.Count)
            {
                messages.Add("Simulation failed!");
            }
            else
            {
                for(var index = 0; index < result.total_workload.Count; index++)
                {
                    string message = "index:" + index.ToString() +
                                     " workload:" + result.total_workload[index] +
                                     " num_bookings:" + result.num_generated_bookings[index] +
                                     " num_unsatisfiable:" + result.num_unsatisfiable_bookings[index];
                    messages.Add(message);
                }
            }

            foreach (var message in messages)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                var array_segment = new ArraySegment<byte>(bytes);
                await web_socket.SendAsync(array_segment, WebSocketMessageType.Text, true, CancellationToken.None);
                Thread.Sleep(100);
            }

            await web_socket.SendAsync(new ArraySegment<byte>(null), WebSocketMessageType.Binary, false,
                CancellationToken.None);
        }

        /// <summary>
        /// Evaluate the just running simulation (Automatically redirect after simulation finished)
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Evaluate(SimulationResult result)
        {
            return View(result);
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