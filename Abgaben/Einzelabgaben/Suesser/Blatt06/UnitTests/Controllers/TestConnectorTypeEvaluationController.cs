using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;

namespace UnitTest.Controller
{
    /// <summary>Test Class for ConnectorTypeEvaluation Controller</summary>
    public class TestConnectorTypeEvaluationController
    {

        private EvaluationController eval;
        private IMemoryCache cache;

        /// <summary>
        /// Setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            eval = new EvaluationController(cache);
        }


        /// <summary>
        /// Test the EvaluationController
        /// </summary>
        [Test]
        public void TestEvaluationController()
        {
            BookingController book = new BookingController(cache);

            //Add Bookings with different Evaluation
            book.Create(new Booking
            {
                s_type = Booking.Steckertyp.CHAdeMO_Plug
            });
            book.Create(new Booking
            {
                s_type = Booking.Steckertyp.Type_1_Plug
            });

            // Check if the percentages are correctly calculated
            var result = eval.Index() as ViewResult;
            var model = (List<ConnectorTypeEvaluationViewModel>)result.Model;
            Assert.AreEqual(50, model.Find(x => x.type == Booking.Steckertyp.CHAdeMO_Plug).percentage);
            Assert.AreEqual(50, model.Find(x => x.type == Booking.Steckertyp.Type_1_Plug).percentage);
        }
    }
}
