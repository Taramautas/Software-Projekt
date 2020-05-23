using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;

namespace Uebungsprojekt.Controllers
{
    public class ConnectorTypeEvaluationController : Controller
    {
        private IMemoryCache _cache;
        List<ConnectorTypeEvaluationViewModel> EvaluationList;
        List<Booking> bookingList = new List<Booking>();
        public ConnectorTypeEvaluationController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public IActionResult Index()
        {
            UpdateEvaluationList();
            return View(EvaluationList);
        }

        private void UpdateEvaluationList()
        {
            EvaluationList = new List<ConnectorTypeEvaluationViewModel>();
            foreach (var Plug in Enum.GetValues(typeof(Booking.ConnectorType)).Cast<Booking.ConnectorType>())
            {
                EvaluationList.Add(new ConnectorTypeEvaluationViewModel
                {
                    ConType = Plug,
                    Percentage = 0.0
                });
            }
            bookingList = (List<Booking>)_cache.Get("booking");
            if (bookingList == null) return;
            foreach (var booking in bookingList)
            {
                foreach (var Eval in EvaluationList)
                {
                    if (booking.Plug == Eval.ConType)
                    {
                        Eval.Percentage += 1.0;
                    }
                }
            }
            if (bookingList.Count == 0) return;
            foreach (var Evaluation in EvaluationList)
            {
                Evaluation.Percentage = Math.Round((Evaluation.Percentage / bookingList.Count) * 100, 2);
            }
        }
    }
}
