using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uebungsprojekt.ViewModel;
using Uebungsprojekt.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{

    public class EvaluationController : Controller
    {

        List<ConnectorTypeEvaluationViewModel> evaluationList;
        List<Booking> bookingList = new List<Booking>();
        private IMemoryCache _cache;


        public EvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            updateEvaluationList();
            return View(evaluationList);
        }

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
    }
}
