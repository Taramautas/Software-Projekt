using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel;
using Uebungsprojekt.ViewModel.Administration;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class TestAdministrationController
    {
        private AdministrationController ac;
        /// <summary>
        /// Setup function executed once before every test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var mock_user_manager = new Mock<UserManager>(new object());
            
            var mock_http_context_accessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();

            mock_http_context_accessor.Setup(_ => _.HttpContext).Returns(context);

            ac = new AdministrationController(mock_user_manager.Object, mock_http_context_accessor.Object);
        }

        [Test]
        public void TestIndex()
        {
            Assert.NotNull(ac.Index());
        }
        
        [Test]
        public void TestSimulationConfig()
        {
            var result = ac.SimulationConfig() as ViewResult;
            var model = (SimulationConfigViewModel) result.Model;
            /*
             * TODO:
             * Assert.AreEqual(SimulationConfigDaoImpl.GetDao().GetAll(), model.all_simulation_configs, 
                "The View Model should contain all existing simulation configs to choose from.");
             */
            Assert.NotNull(model.all_simulation_configs);
        }
        
        [Test]
        public void TestSimulationInfrastructure()
        {
            SimulationInfrastructureViewModel view_model = new SimulationInfrastructureViewModel();
            Assert.NotNull(ac.SimulationInfrastructure(view_model));
        }
        
        [Test]
        public void TestSimulationExists()
        {
            SimulationViewModel view_model = GetSimulationViewModel();
            Assert.NotNull(ac.Simulation(view_model));
        }
        
        [Test]
        public void TestSimulationCreatesResult()
        {
            SimulationViewModel view_model = GetSimulationViewModel();
            var result = ac.Simulation(view_model) as ViewResult;
            var model = (int)result.Model;
        }
        
        [Test]
        public void TestEvaluateExists()
        {
            SimulationViewModel simulation_view_model = GetSimulationViewModel();
            SimulationResult view_model = new SimulationResult(simulation_view_model.simulation_config,
                simulation_view_model.simulation_infrastructure);
            var result = ac.Evaluate(view_model) as ViewResult;
            var model = (SimulationResult)result.Model;
        }

        private SimulationViewModel GetSimulationViewModel()
        {
            SimulationViewModel view_model = new SimulationViewModel();
            view_model.simulation_config = new SimulationConfig()
            {
                id = 0,
                max = 10,
                min = 3,
                spread = 40.0,
                tick_minutes = 5,
                weeks = 2,
                vehicles = new List<Vehicle>()
                {
                    new Vehicle()
                    {
                        capacity = 460,
                        connector_types = new List<ConnectorType>()
                        {
                            ConnectorType.Schuko_Socket,
                            ConnectorType.Tesla_Supercharger
                        },
                        id = 1,
                        model_name = "Tesla"
                    },
                    new Vehicle()
                    {
                        capacity = 410,
                        connector_types = new List<ConnectorType>()
                        {
                            ConnectorType.Type_1_Plug,
                            ConnectorType.Type_2_Plug
                        },
                        id = 2,
                        model_name = "Foo"
                    },
                },
                rush_hours = new List<Tuple<DayOfWeek, TimeSpan>>()
                {
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Monday, new TimeSpan(7, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Tuesday, new TimeSpan(7, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Wednesday, new TimeSpan(7, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Thursday, new TimeSpan(7, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Friday, new TimeSpan(7, 0, 0)),
                    
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Monday, new TimeSpan(16, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Tuesday, new TimeSpan(16, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Wednesday, new TimeSpan(16, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Thursday, new TimeSpan(16, 0, 0)),
                    new Tuple<DayOfWeek, TimeSpan>(DayOfWeek.Friday, new TimeSpan(12, 30, 0)),
                }
            };
            view_model.simulation_infrastructure = new SimulationInfrastructure()
            {
                booking_dao = new object(),
                location_dao = new object(),
                charging_zone_dao = new object(),
                charging_column_dao = new object(),
                id = 1,
            };
            return view_model;
        }
    }
}