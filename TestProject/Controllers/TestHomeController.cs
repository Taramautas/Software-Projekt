using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Uebungsprojekt.Controllers;

namespace UnitTest.Controllers
{
    /// <summary>Test Class for Home Controller</summary>
    class TestHomeController
    {
        private HomeController hc;

        /// <summary>
        /// Setup function executed once before every test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var user_manager = new Mock<UserManager>();
            hc = new HomeController(user_manager.Object);
        }
    }
}
