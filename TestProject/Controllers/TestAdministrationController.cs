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
        public void TestSimulationInfrastructure()
        {
            SimulationInfrastructureViewModel view_model = new SimulationInfrastructureViewModel();
            Assert.NotNull(ac.SimulationInfrastructure(view_model));
        }
    }
}