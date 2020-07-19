using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestSimulationConfigDaoImpl
    {
        private SimulationConfigDaoImpl _simulationConfigDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _simulationConfigDao = new SimulationConfigDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditSimulationConfigChagesObjectInCache()
        {
            // Initialize Daos
            List<SimulationConfig> simulationConfigList1 = _simulationConfigDao.GetAll();

            // Create new SimulationConfigs
            int simulationConfig_id11 = _simulationConfigDao.Create(10, new List<Tuple<DayOfWeek, TimeSpan>>(), 1, 9, 3, 2, new List<Vehicle>());

            // Edit SimulationConfig in Dao1 with GetById
            Assert.AreEqual(10, _simulationConfigDao.GetById(simulationConfig_id11).tick_minutes);
            SimulationConfig simulationConfig11 = _simulationConfigDao.GetById(simulationConfig_id11);
            simulationConfig11.tick_minutes = 9;

            // check if the simulationConfig got changed in the cache
            simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(9, _simulationConfigDao.GetById(simulationConfig_id11).tick_minutes);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectSimulationConfig()
        {
            // Initialize Daos
            _simulationConfigDao.GetAll();


            // Create new simulationConfigs in List1 and add it with Create
            SimulationConfig simulationConfig11 = new SimulationConfig
            {
                id = 1,
                tick_minutes = 10,
                min = 1,
                max = 9,
            };
            SimulationConfig simulationConfig21 = new SimulationConfig
            {
                id = 2,
                tick_minutes = 11,
                min = 1,
                max = 9,
            };
            int bk_id11 = _simulationConfigDao.Create(simulationConfig11.tick_minutes, new List<Tuple<DayOfWeek, TimeSpan>>(), simulationConfig11.min, simulationConfig11.max, 3, 2, new List<Vehicle>());
            int bk_id21 = _simulationConfigDao.Create(simulationConfig21.tick_minutes, new List<Tuple<DayOfWeek, TimeSpan>>(), simulationConfig21.min, simulationConfig21.max, 3, 2, new List<Vehicle>());

            // Check if GetById returns the correct simulationConfig
            Assert.AreEqual(simulationConfig11.tick_minutes, _simulationConfigDao.GetById(bk_id11).tick_minutes);

            Assert.AreEqual(simulationConfig21.tick_minutes, _simulationConfigDao.GetById(bk_id21).tick_minutes);

        }

        
        /// <summary>
        /// Tests if simulationConfigs added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewSimulationConfigToCache()
        {
            // Check if initial amount of simulationConfigs is 0
            List<SimulationConfig> simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(0, simulationConfigList1.Count);

            // Create new simulationConfig in List1 and add it with Create
            _simulationConfigDao.Create(
                11,
                new List<Tuple<DayOfWeek, TimeSpan>>(),
                1,
                9,
                3,
                2,
                new List<Vehicle>()
                );

            // Check if there is 1 SimulationConfig in simulationConfigList1
            simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(1, simulationConfigList1.Count);


            // Check again with 2 in simulationConfigList1
            simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(1, simulationConfigList1.Count);

            // Create new simulationConfig in List1 and add it with Create
            _simulationConfigDao.Create(
                12,
                new List<Tuple<DayOfWeek, TimeSpan>>(),
                1,
                9,
                3,
                2,
                new List<Vehicle>()
                );


            // Check if there is 2 simulationConfigs in simulationConfigList1
            simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(2, simulationConfigList1.Count);
        }
        
        
        
        /// <summary>
        /// Tests if simulationConfigs added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewSimulationConfigFromCache()
        {
            // Check if initial amount of simulationConfigs is 0
            List<SimulationConfig> simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(0, simulationConfigList1.Count);

            // Create new simulationConfig in List1 and add it with Create
            int bk_id11 = _simulationConfigDao.Create(
                12,
                new List<Tuple<DayOfWeek, TimeSpan>>(),
                1,
                9,
                3,
                2,
                new List<Vehicle>()
                );

            // Create new simulationConfig in List1 and add it with Create
            int bk_id12 = _simulationConfigDao.Create(
                13,
                new List<Tuple<DayOfWeek, TimeSpan>>(),
                1,
                9,
                3,
                2,
                new List<Vehicle>()
                );


            // Check if there is 2 simulationConfigs in simulationConfigList1
            simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(2, simulationConfigList1.Count);

            // Delete the first simulationConfig from simulationConfigList1
            _simulationConfigDao.Delete(bk_id11);

            // Check if there is 1 simulationConfig in simulationConfigList1
            simulationConfigList1 = _simulationConfigDao.GetAll();
            Assert.AreEqual(1, simulationConfigList1.Count);

            // Check if the second SimulationConfig really is the correct one left
            Assert.AreEqual(13, _simulationConfigDao.GetById(bk_id12).tick_minutes);
        }
    }
}