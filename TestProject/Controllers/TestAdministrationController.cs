using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NUnit.Framework;
using Uebungsprojekt;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel.Home;

namespace UnitTest.Controllers.Administration
{
    [TestFixture]
    public class TestAdministrationControllerSimulationMethodsExists
    {
        private AdministrationController ac;
        /// <summary>
        /// Setup function executed once before every test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Setup controller with initalized request parameters
            var context_accessor = new HttpContextAccessor();
            context_accessor.HttpContext = new DefaultHttpContext();
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            ac = new AdministrationController(new UserManager(cache), cache, context_accessor);

            ac.ControllerContext = new ControllerContext();
            ac.ControllerContext.HttpContext = context_accessor.HttpContext;
            ac.ControllerContext.HttpContext.Request.Headers["init"] = "0";
        }
        /*
        [Test]
        public void TestSimulationConfigExists()
        {
            Assert.NotNull(ac.SimulationConfig(new SimulationConfig()));
        }

        [Test]
        public void TestSimulationInfrastructureExists()
        {
            Assert.NotNull(ac.SimulationInfrastructure());
        }

        [Test]
        public void TestSimulationExists()
        {
            Assert.NotNull(ac.Simulation());
        }

        [Test]
        public void TestSimulationEvaluationExists()
        {
            Assert.NotNull(ac.Simulation());
        }

        [Test]
        public void TestCreateSimulationLocationExists()
        {
            Assert.NotNull(ac.CreateSimulationLocation());
        }

        [Test]
        public void TestDeleteSimulationLocationExists()
        {
            Assert.NotNull(ac.DeleteSimulationLocation(1));
        }

        [Test]
        public void TestCreateSimulationChargingZoneExists()
        {
            Assert.NotNull(ac.CreateSimulationChargingZone());
        }

        [Test]
        public void TestDeleteSimulationChargingZoneExists()
        {
            Assert.NotNull(ac.DeleteSimulationChargingZone(1));
        }

        [Test]
        public void TestAddSimulationChargingColumnExists()
        {
            Assert.NotNull(ac.AddSimulationChargingColumn());
        }

        [Test]
        public void TestDeleteSimulationChargingColumnExists()
        {
            Assert.NotNull(ac.DeleteSimulationChargingColumn(1));
        }

        [Test]
        public void TestAddSimulationVehicleExists()
        {
            Assert.NotNull(ac.AddSimulationVehicle());
        }
        
        [Test]
        public void TestDeleteSimulationVehicleExists()
        {
            Assert.NotNull(ac.DeleteSimulationVehicle(1));
        }
        
        [Test]
        public void TestDeleteAllSimulationVehiclesExists()
        {
            Assert.NotNull(ac.DeleteAllSimulationVehicles());
        }

        [Test]
        public void TestAddRushHoursExists()
        {
            Assert.NotNull(ac.AddRushHours());
        }

        [Test]
        public void TestDeleteRushHoursExists()
        {
            Assert.NotNull(ac.DeleteRushHours(1));
        }
    }

    public class TestAdministrationControllerMethodsExists
    {
        private AdministrationController ac;

        [SetUp]
        public void Setup()
        {
            // Setup controller with initalized request parameters
            var context_accessor = new HttpContextAccessor();
            context_accessor.HttpContext = new DefaultHttpContext();
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            ac = new AdministrationController(new UserManager(cache), cache, context_accessor);

            ac.ControllerContext = new ControllerContext();
            ac.ControllerContext.HttpContext = context_accessor.HttpContext;
            ac.ControllerContext.HttpContext.Request.Headers["init"] = "0";
        }


        [Test]
        public void TestIndexExists()
        {
            Assert.NotNull(ac.Index());
        }

        [Test]
        public void TestInfrastructureExists()
        {
            Assert.NotNull(ac.Infrastructure());
        }

        [Test]
        public void TestCreateLocationExists()
        {
            Assert.NotNull(ac.CreateLocation());
        }

        [Test]
        public void TestDeleteLocationExists()
        {
            Assert.NotNull(ac.DeleteLocation(1));
        }

        [Test]
        public void TestCreateChargingZoneExists()
        {
            Assert.NotNull(ac.CreateChargingZone());
        }

        [Test]
        public void TestDeleteChargingZoneExists()
        {
            Assert.NotNull(ac.DeleteChargingZone(1));
        }

        [Test]
        public void TestAddChargingColumnExists()
        {
            Assert.NotNull(ac.AddChargingColumn());
        }

        [Test]
        public void TestDeleteChargingColumnExists()
        {
            Assert.NotNull(ac.DeleteChargingColumn(1));
        }

        [Test]
        public void TestVehiclesExists()
        {
            Assert.NotNull(ac.Vehicles());
        }

        [Test]
        public void TestCreateVehicleExists()
        {
            Assert.NotNull(ac.CreateVehicle());
        }

        [Test]
        public void TestDeleteVehicleExists()
        {
            Assert.NotNull(ac.DeleteVehicle(1));
        }

        [Test]
        public void TestBookingsExists()
        {
            Assert.NotNull(ac.Bookings());
        }

        [Test]
        public void TestCreateBookingExists()
        {
            Assert.NotNull(ac.CreateBooking());
        }
        
        [Test]
        public void TestDeleteBookingExists()
        {
            Assert.NotNull(ac.Delete(1));
        }

        [Test]
        public void TestEditBookingExists()
        {
            Assert.NotNull(ac.EditBooking(1));

        }

        [Test]
        public void TestChargingColumnTypeExists()
        {
            Assert.NotNull(ac.ChargingColumnType());
        }

        [Test]
        public void TestCreateChargingColumnTypeExists()
        {
            Assert.NotNull(ac.CreateChargingColumnType());
        }
        
        [Test]
        public void TestDeleteChargingColumnTypeExists()
        {
            Assert.NotNull(ac.DeleteChargingColumnType(1));
        }
        
        [Test]
        public void TestImportEverythingExists()
        {
            Assert.NotNull(ac.ImportEverything(new List<IFormFile>()));
        }
        
        [Test]
        public void TestExportEverythingExists()
        {
            Assert.NotNull(ac.ExportEverything());
        }
    }

    [TestFixture]
    public class TestAdministrationControllerSimulationPathsExist
    {
        private WebApplicationFactory<Startup> factory;
        private HttpClient client;

        [SetUp]
        public void Setup()
        {
            // Setup controller with initalized request parameters
            factory = new WebApplicationFactory<Startup>();
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost:44394"),
            });
        }

        [Test]
        public async Task TestSimulationConfigPathExists()
        {
            var response = await client.GetAsync("/Administration/SimulationConfig");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestSimulationInfrastructurePathExists()
        {
            var response = await client.GetAsync("/Administration/SimulationInfrastructure");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestSimulationPathExists()
        {
            var response = await client.GetAsync("/Administration/Simulation");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestSimulationEvaluationPathExists()
        {
            var response = await client.GetAsync("/Administration/SimulationEvaluation");
            response.EnsureSuccessStatusCode();
        }


        [Test]
        public async Task TestCreateSimulationLocationPathExists()
        {
            var response = await client.GetAsync("/Administration/CreateSimulationLocation");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteSimulationLocationPathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteSimulationLocation");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestCreateSimulationChargingZonePathExists()
        {
            var response = await client.GetAsync("/Administration/CreateSimulationChargingZone");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteSimulationChargingZonePathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteSimulationChargingZone");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestAddSimulationChargingColumnPathExists()
        {
            var response = await client.GetAsync("/Administration/AddSimulationChargingColumn");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteSimulationChargingColumnPathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteSimulationChargingColumn");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestAddSimulationVehiclePathExists()
        {
            var response = await client.GetAsync("/Administration/AddSimulationVehicle");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteSimulationVehiclePathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteSimulationVehicle");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteAllSimulationVehiclesPathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteAllSimulationVehicles");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestAddRushHoursPathExists()
        {
            var response = await client.GetAsync("/Administration/AddRushHours");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteRushHoursExists()
        {
            var response = await client.GetAsync("/Administration/DeleteRushHours");
            response.EnsureSuccessStatusCode();
        }
    }

    [TestFixture]
    public class TestAdministrationControllerPathsExist
    {
        private WebApplicationFactory<Startup> factory;
        private HttpClient client;

        [SetUp]
        public void Setup()
        {
            // Setup controller with initalized request parameters
            factory = new WebApplicationFactory<Startup>();
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost:44394"),
            });
        }

        [Test]
        public async Task TestIndexPathExists()
        {
            var response = await client.GetAsync("/Administration/Index");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestInfrastructurePathExists()
        {
            var response = await client.GetAsync("/Administration/Infrastructure");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestCreateLocationPathExists()
        {
            var response = await client.GetAsync("/Administration/CreateLocation");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteLocationPathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteLocation");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestCreateChargingZonePathExists()
        {
            var response = await client.GetAsync("/Administration/CreateChargingZone");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteChargingZonePathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteChargingZone");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestAddChargingColumnPathExists()
        {
            var response = await client.GetAsync("/Administration/AddChargingColumn");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteChargingColumnPathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteChargingColumn");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestBookingsPathExists()
        {
            var response = await client.GetAsync("/Administration/Bookings");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestCreateBookingPathExists()
        {
            var response = await client.GetAsync("/Administration/CreateBooking");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteBookingPathExists()
        {
            var response = await client.GetAsync("/Administration/Delete");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestEditBookingPathExists()
        {
            var response = await client.GetAsync("/Administration/EditBooking");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestVehiclesPathExists()
        {
            var response = await client.GetAsync("/Administration/Vehicles");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestCreateVehiclePathExists()
        {
            var response = await client.GetAsync("/Administration/CreateVehicle");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteVehiclePathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteVehicle");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestChargingColumnTypePathExists()
        {
            var response = await client.GetAsync("/Administration/ChargingColumnType");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestCreateChargingColumnTypePathExists()
        {
            var response = await client.GetAsync("/Administration/CreateChargingColumnType");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestDeleteChargingColumnTypePathExists()
        {
            var response = await client.GetAsync("/Administration/DeleteChargingColumnType");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task TestImportEverythingPathExists()
        {
            var response = await client.PostAsync("/Administration/ImportEverything", new StringContent(""));
            response.EnsureSuccessStatusCode();
        }
        
        [Test]
        public async Task TestExportEverythingPathExists()
        {
            var response = await client.GetAsync("/Administration/ExportEverything");
            response.EnsureSuccessStatusCode();
        }
        
        [Test]
        public async Task TestWrongThrowsHttpRequestException()
        {
            var response = await client.GetAsync("/Administration/WrongPath");
            Assert.Throws<HttpRequestException>( () => response.EnsureSuccessStatusCode());
        }
    }

    [TestFixture, SingleThreaded]
    public class TestAdministrationControllerMethodsWork
    {
        private WebApplicationFactory<Startup> factory;
        private HttpClient client;
        private int run = 0;
        [SetUp]
        public void Setup()
        {
            // Setup controller with initalized request parameters
           // factory = new WebApplicationFactory<Startup>();
            factory = new WebApplicationFactory<Startup>();
            client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                   BaseAddress = new Uri("https://localhost:44394"),
            });
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task TestNoAccessAsNotVipAssistantGuestUser_()
        {
            await IntegrationTestHelper.Login(client, "user@user.de", "user");
            var response = await client.GetAsync("/Administration/Index");
            Console.WriteLine(response);
            Assert.AreEqual("https://localhost:44394/Home/Error/?ReturnUrl=%2FAdministration%2FIndex", response.RequestMessage.RequestUri.ToString());
        }
        
        
        [Test]
        public async Task TestNoAccessAsNotVipAssistantGuestUsers()
        {
            await IntegrationTestHelper.Login(client, "assistant@assistant.de", "assistant");
            var response = await client.GetAsync("/Administration/Index");
            Assert.AreEqual("https://localhost:44394/Home/Error/?ReturnUrl=%2FAdministration%2FIndex", response.RequestMessage.RequestUri.ToString());
        }
        
        [Test]
        public async Task TestHasAccessAsPlanner()
        {
            await IntegrationTestHelper.Login(client, "admin@admin.de", "admin");
            var response = await client.GetAsync("/Administration/Index");
            Assert.AreEqual("https://localhost:44394/Administration/Index", response.RequestMessage.RequestUri.ToString());
        }
        

        [Test]
        public async Task TestImportExport()
        {
            var response = await client.GetAsync("/Administration/ExportEverything");
            response.EnsureSuccessStatusCode();
            HttpContent first_result = response.Content;

            response = await client.PostAsync("/Administration/ImportEverything", first_result);
            response.EnsureSuccessStatusCode();
            
            response = await client.GetAsync("/Administration/ExportEverything");
            response.EnsureSuccessStatusCode();
            HttpContent third_result = response.Content;

            string first_json = await first_result.ReadAsStringAsync();
            string second_json = await third_result.ReadAsStringAsync();
            
            Assert.AreEqual(first_json, second_json);
        }
        */
    }
}