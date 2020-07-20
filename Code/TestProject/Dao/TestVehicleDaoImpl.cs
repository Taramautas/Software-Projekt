using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestVehicleDaoImpl
    {
        private VehicleDaoImpl _vehicleDao;
        private UserDaoImpl _userDao;

        [SetUp]
        public void Setup()
        {
            _vehicleDao = new VehicleDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditVehicleChagesObjectInCache()
        {
            // Initialize Daos
            List<Vehicle> vehicleList1 = _vehicleDao.GetAll();

            // Create new Vehicles
            int vehicle_id11 = _vehicleDao.Create("Alpha", 100, new List<ConnectorType>(), new User());

            // Edit Vehicle in Dao1 with GetById
            Assert.AreEqual("Alpha", _vehicleDao.GetById(vehicle_id11).model_name);
            Vehicle vehicle11 = _vehicleDao.GetById(vehicle_id11);
            vehicle11.model_name = "Beta";

            // check if the vehicle got changed in the cache
            Assert.AreEqual("Beta", _vehicleDao.GetById(vehicle_id11).model_name);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectVehicle()
        {
            // Initialize Daos
            _vehicleDao.GetAll();


            // Create new vehicles in List1 and add it with Create
            Vehicle vehicle11 = new Vehicle
            {
                id = 1,
                model_name = "Alpha",
                capacity = 100,
                connector_types = new List<ConnectorType>(),
                user = new User()
            };
            Vehicle vehicle21 = new Vehicle
            {
                id = 2,
                model_name = "Beta",
                capacity = 100,
                connector_types = new List<ConnectorType>(),
                user = new User()
            };
            int bk_id11 = _vehicleDao.Create(vehicle11.model_name, vehicle11.capacity, vehicle11.connector_types, vehicle11.user);
            int bk_id21 = _vehicleDao.Create(vehicle21.model_name, vehicle21.capacity, vehicle21.connector_types, vehicle21.user);

            // Check if GetById returns the correct vehicle
            Assert.AreEqual(vehicle11.model_name, _vehicleDao.GetById(bk_id11).model_name);

            Assert.AreEqual(vehicle21.model_name, _vehicleDao.GetById(bk_id21).model_name);

        }

        
        /// <summary>
        /// Tests if vehicles added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewVehicleToCache()
        {
            // Check if initial amount of vehicles is 0
            List<Vehicle> vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(0, vehicleList1.Count);

            // Create new vehicle in List1 and add it with Create
            _vehicleDao.Create(
                "Alpha",
                100,
                new List<ConnectorType>(),
                new User()
                );

            // Check if there is 1 Vehicle in vehicleList1
            vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(1, vehicleList1.Count);


            // Check again with 2 in vehicleList1
            vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(1, vehicleList1.Count);

            // Create new vehicle in List1 and add it with Create
            _vehicleDao.Create(
                "Beta",
                100,
                new List<ConnectorType>(),
                new User()
                );


            // Check if there is 2 vehicles in vehicleList1
            vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(2, vehicleList1.Count);
        }
        
        
        
        /// <summary>
        /// Tests if vehicles added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewVehicleFromCache()
        {
            // Check if initial amount of vehicles is 0
            List<Vehicle> vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(0, vehicleList1.Count);

            // Create new vehicle in List1 and add it with Create
            int bk_id11 = _vehicleDao.Create(
                "Alpha",
                100,
                new List<ConnectorType>(),
                new User()
                );

            // Create new vehicle in List1 and add it with Create
            int bk_id12 = _vehicleDao.Create(
                "Beta",
                100,
                new List<ConnectorType>(),
                new User()
                );


            // Check if there is 2 vehicles in vehicleList1
            vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(2, vehicleList1.Count);

            // Delete the first vehicle from vehicleList1
            _vehicleDao.Delete(bk_id11);

            // Check if there is 1 vehicle in vehicleList1
            vehicleList1 = _vehicleDao.GetAll();
            Assert.AreEqual(1, vehicleList1.Count);

            // Check if the second Vehicle really is the correct one left
            Assert.AreEqual("Beta", _vehicleDao.GetById(bk_id12).model_name);
        }
    }
}