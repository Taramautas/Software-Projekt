using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestSimulationInfrastructureDaoImpl
    {
        private SimulationInfrastructureDaoImpl _simulationInfrastructureDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _simulationInfrastructureDao = new SimulationInfrastructureDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditSimulationInfrastructureChagesObjectInCache()
        {
            // Initialize Daos
            List<SimulationInfrastructure> simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();

            // Create new SimulationInfrastructures
            int simulationInfrastructure_id11 = _simulationInfrastructureDao.Create(1, 1, 1);

            // Edit SimulationInfrastructure in Dao1 with GetById
            Assert.AreEqual(1, _simulationInfrastructureDao.GetById(simulationInfrastructure_id11).location_dao_id);
            SimulationInfrastructure simulationInfrastructure11 = _simulationInfrastructureDao.GetById(simulationInfrastructure_id11);
            simulationInfrastructure11.location_dao_id = 2;

            // check if the simulationInfrastructure got changed in the cache
            simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(2, _simulationInfrastructureDao.GetById(simulationInfrastructure_id11).location_dao_id);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectSimulationInfrastructure()
        {
            // Initialize Daos
            _simulationInfrastructureDao.GetAll();


            // Create new simulationInfrastructures in List1 and add it with Create
            SimulationInfrastructure simulationInfrastructure11 = new SimulationInfrastructure
            {
                id = 1,
                location_dao_id = 1,
                charging_zone_dao_id = 1,
                charging_column_dao_id = 1,
            };
            SimulationInfrastructure simulationInfrastructure21 = new SimulationInfrastructure
            {
                id = 2,
                location_dao_id = 2,
                charging_zone_dao_id = 2,
                charging_column_dao_id = 2,
            };
            int bk_id11 = _simulationInfrastructureDao.Create(simulationInfrastructure11.location_dao_id, simulationInfrastructure11.charging_zone_dao_id, simulationInfrastructure11.charging_column_dao_id);
            int bk_id21 = _simulationInfrastructureDao.Create(simulationInfrastructure21.location_dao_id, simulationInfrastructure21.charging_zone_dao_id, simulationInfrastructure21.charging_column_dao_id);

            // Check if GetById returns the correct simulationInfrastructure
            Assert.AreEqual(simulationInfrastructure11.location_dao_id, _simulationInfrastructureDao.GetById(bk_id11).location_dao_id);

            Assert.AreEqual(simulationInfrastructure21.location_dao_id, _simulationInfrastructureDao.GetById(bk_id21).location_dao_id);

        }

        
        /// <summary>
        /// Tests if simulationInfrastructures added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewSimulationInfrastructureToCache()
        {
            // Check if initial amount of simulationInfrastructures is 0
            List<SimulationInfrastructure> simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(0, simulationInfrastructureList1.Count);

            // Create new simulationInfrastructure in List1 and add it with Create
            _simulationInfrastructureDao.Create(
                1,
                1,
                1
                );

            // Check if there is 1 SimulationInfrastructure in simulationInfrastructureList1
            simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(1, simulationInfrastructureList1.Count);


            // Check again with 2 in simulationInfrastructureList1
            simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(1, simulationInfrastructureList1.Count);

            // Create new simulationInfrastructure in List1 and add it with Create
            _simulationInfrastructureDao.Create(
                2,
                2,
                2
                );


            // Check if there is 2 simulationInfrastructures in simulationInfrastructureList1
            simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(2, simulationInfrastructureList1.Count);
        }
        
        
        
        /// <summary>
        /// Tests if simulationInfrastructures added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewSimulationInfrastructureFromCache()
        {
            // Check if initial amount of simulationInfrastructures is 0
            List<SimulationInfrastructure> simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(0, simulationInfrastructureList1.Count);

            // Create new simulationInfrastructure in List1 and add it with Create
            int bk_id11 = _simulationInfrastructureDao.Create(
                1,
                1,
                1
                );

            // Create new simulationInfrastructure in List1 and add it with Create
            int bk_id12 = _simulationInfrastructureDao.Create(
                2,
                2,
                2
                );


            // Check if there is 2 simulationInfrastructures in simulationInfrastructureList1
            simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(2, simulationInfrastructureList1.Count);

            // Delete the first simulationInfrastructure from simulationInfrastructureList1
            _simulationInfrastructureDao.Delete(bk_id11);

            // Check if there is 1 simulationInfrastructure in simulationInfrastructureList1
            simulationInfrastructureList1 = _simulationInfrastructureDao.GetAll();
            Assert.AreEqual(1, simulationInfrastructureList1.Count);

            // Check if the second SimulationInfrastructure really is the correct one left
            Assert.AreEqual(2, _simulationInfrastructureDao.GetById(bk_id12).location_dao_id);
        }
    }
}