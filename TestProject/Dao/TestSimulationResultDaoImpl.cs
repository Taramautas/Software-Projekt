using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestSimulationResultDaoImpl
    {
        private SimulationResultDaoImpl _simulationResultDao;
        private UserDaoImpl _userDao;

        [SetUp]
        public void Setup()
        {
            _simulationResultDao = new SimulationResultDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditSimulationResultChagesObjectInCache()
        {
            // Initialize Daos
            List<SimulationResult> simulationResultList1 = _simulationResultDao.GetAll();

            // Create new SimulationResults
            int simulationResult_id11 = _simulationResultDao.Create(new SimulationConfig { tick_minutes = 10 }, new SimulationInfrastructure(), new List<Dictionary<int, double>>(), new List<int>(), new List<int>(), true);

            // Edit SimulationResult in Dao1 with GetById
            Assert.AreEqual(10, _simulationResultDao.GetById(simulationResult_id11).config.tick_minutes);
            SimulationResult simulationResult11 = _simulationResultDao.GetById(simulationResult_id11);
            simulationResult11.config.tick_minutes = 11;

            // check if the simulationResult got changed in the cache
            Assert.AreEqual(11, _simulationResultDao.GetById(simulationResult_id11).config.tick_minutes);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectSimulationResult()
        {
            // Initialize Daos
            _simulationResultDao.GetAll();


            // Create new simulationResults in List1 and add it with Create
            SimulationResult simulationResult11 = new SimulationResult
            {
                id = 1,
                config = new SimulationConfig { tick_minutes = 10 },
            };
            SimulationResult simulationResult21 = new SimulationResult
            {
                id = 2,
                config = new SimulationConfig { tick_minutes = 11 },
            };
            int bk_id11 = _simulationResultDao.Create(simulationResult11.config, new SimulationInfrastructure(), new List<Dictionary<int, double>>(), new List<int>(), new List<int>(), true);
            int bk_id21 = _simulationResultDao.Create(simulationResult21.config, new SimulationInfrastructure(), new List<Dictionary<int, double>>(), new List<int>(), new List<int>(), true);

            // Check if GetById returns the correct simulationResult
            Assert.AreEqual(simulationResult11.config.tick_minutes, _simulationResultDao.GetById(bk_id11).config.tick_minutes);

            Assert.AreEqual(simulationResult21.config.tick_minutes, _simulationResultDao.GetById(bk_id21).config.tick_minutes);

        }

        
        /// <summary>
        /// Tests if simulationResults added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewSimulationResultToCache()
        {
            // Check if initial amount of simulationResults is 0
            List<SimulationResult> simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(0, simulationResultList1.Count);

            // Create new simulationResult in List1 and add it with Create
            _simulationResultDao.Create(
                new SimulationConfig { tick_minutes = 10 },
                new SimulationInfrastructure(), 
                new List<Dictionary<int, double>>(), 
                new List<int>(), 
                new List<int>(), 
                true
                );

            // Check if there is 1 SimulationResult in simulationResultList1
            simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(1, simulationResultList1.Count);


            // Check again with 2 in simulationResultList1
            simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(1, simulationResultList1.Count);

            // Create new simulationResult in List1 and add it with Create
            _simulationResultDao.Create(
                new SimulationConfig { tick_minutes = 11 },
                new SimulationInfrastructure(),
                new List<Dictionary<int, double>>(),
                new List<int>(),
                new List<int>(),
                true
                );


            // Check if there is 2 simulationResults in simulationResultList1
            simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(2, simulationResultList1.Count);
        }
        
        
        
        /// <summary>
        /// Tests if simulationResults added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewSimulationResultFromCache()
        {
            // Check if initial amount of simulationResults is 0
            List<SimulationResult> simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(0, simulationResultList1.Count);

            // Create new simulationResult in List1 and add it with Create
            int bk_id11 = _simulationResultDao.Create(
                new SimulationConfig { tick_minutes = 10 },
                new SimulationInfrastructure(),
                new List<Dictionary<int, double>>(),
                new List<int>(),
                new List<int>(),
                true
                );

            // Create new simulationResult in List1 and add it with Create
            int bk_id12 = _simulationResultDao.Create(
                new SimulationConfig { tick_minutes = 11 },
                new SimulationInfrastructure(),
                new List<Dictionary<int, double>>(),
                new List<int>(),
                new List<int>(),
                true
                );


            // Check if there is 2 simulationResults in simulationResultList1
            simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(2, simulationResultList1.Count);

            // Delete the first simulationResult from simulationResultList1
            _simulationResultDao.Delete(bk_id11);

            // Check if there is 1 simulationResult in simulationResultList1
            simulationResultList1 = _simulationResultDao.GetAll();
            Assert.AreEqual(1, simulationResultList1.Count);

            // Check if the second SimulationResult really is the correct one left
            Assert.AreEqual(11, _simulationResultDao.GetById(bk_id12).config.tick_minutes);
        }
    }
}