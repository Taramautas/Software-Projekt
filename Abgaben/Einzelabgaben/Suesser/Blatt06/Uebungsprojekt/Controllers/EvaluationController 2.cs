using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.ViewModel;
using Uebungsprojekt.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Uebungsprojekt.Controllers
{
    /// <summary>
    /// Controller for Evaluation ViewModel
    /// </summary>
    public class EvaluationController : Controller
    {

        List<ConnectorTypeEvaluationViewModel> evaluationList;
        List<Booking> bookingList = new List<Booking>();
        private IMemoryCache _cache;

        /// <summary>
        /// Constructor of controller. Initialize the memory cache
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for intializing the memory cache</param>
        public EvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }


        /// <summary>
        /// Displays the Evaluation View and passes the evaluation list initialized in the constructor. 
        /// </summary>
        /// <returns>
        /// The Evaluation View displaying the list of connector-types and percentages
        /// </returns>
        public IActionResult Index()
        {
            updateEvaluationList();
            return View(evaluationList);
        }

        /// <summary>
        /// Updates the EvaluationList with correct percentages
        /// </summary>
        private void updateEvaluationList()
        {
            evaluationList = new List<ConnectorTypeEvaluationViewModel>();
            foreach (var s_type in Enum.GetValues(typeof(Booking.Steckertyp)).Cast<Booking.Steckertyp>())
            {
                evaluationList.Add(new ConnectorTypeEvaluationViewModel
                {
                    type = s_type,
                    percentage = 0.0
                });
            }
            bookingList = (List<Booking>)_cache.Get("booking");
            if (bookingList == null) return;

            foreach(var booking in bookingList)
            {
                foreach(var eval in evaluationList)
                {
                    if(booking.s_type == eval.type)
                    {
                        eval.percentage +=1.0;
                    }
                }
            }
            if (bookingList.Count == 0) return;
            foreach(var eval in evaluationList)
            {
                eval.percentage = Math.Round((eval.percentage / bookingList.Count) * 100, 2);
            }
        }

        public IActionResult Export()
        {
            updateEvaluationList();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evaluationList));
            var output = new FileContentResult(bytes, "application/octet-stream");
            output.FileDownloadName = "Evaluation.json";
            return output;
        }
    }
}
