using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestChargingColumnTypeTypeDaoImpl
    {
        private ChargingColumnTypeDaoImpl _chargingColumnTypeDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _chargingColumnTypeDao = new ChargingColumnTypeDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditChargingColumnTypeChagesObjectInCache()
        {
            // Initialize Daos with IDs
            List<ChargingColumnType> chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();

            // Create new ChargingColumnTypes
            int chargingColumnType_id11 = _chargingColumnTypeDao.Create("model A", "manufactuerer A", 2, new List<Tuple<ConnectorType, int>>());

            // Edit ChargingColumnType in Dao1 with GetById
            Assert.AreEqual("model A", _chargingColumnTypeDao.GetById(chargingColumnType_id11).model_name);
            ChargingColumnType chargingColumnType11 = _chargingColumnTypeDao.GetById(chargingColumnType_id11);
            chargingColumnType11.model_name = "model B";

            // check if the chargingColumnType got changed in the cache
            Assert.AreEqual("model B", _chargingColumnTypeDao.GetById(chargingColumnType_id11).model_name);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectChargingColumnType()
        {
            // Initialize Daos with IDs
            _chargingColumnTypeDao.GetAll();
            _chargingColumnTypeDao.GetAll();


            // Create new chargingColumnTypes in List1 and add it with Create
            ChargingColumnType chargingColumnType11 = new ChargingColumnType
            {
                id = 1,
                model_name = "model A",
                manufacturer_name = "manufacturer A",
                max_parallel_charging = 2,
                connectors = new List<Tuple<ConnectorType, int>>(),
            };
            ChargingColumnType chargingColumnType21 = new ChargingColumnType
            {
                id = 2,
                model_name = "model B",
                manufacturer_name = "manufacturer B",
                max_parallel_charging = 3,
                connectors = new List<Tuple<ConnectorType, int>>(),
            };
            int bk_id11 = _chargingColumnTypeDao.Create(chargingColumnType11.model_name, chargingColumnType11.manufacturer_name, chargingColumnType11.max_parallel_charging, chargingColumnType11.connectors);
            int bk_id21 = _chargingColumnTypeDao.Create(chargingColumnType21.model_name, chargingColumnType21.manufacturer_name, chargingColumnType21.max_parallel_charging, chargingColumnType21.connectors);

            // Create new chargingColumnTypes in List2 and add it with Create
            ChargingColumnType chargingColumnType12 = new ChargingColumnType
            {
                id = 1,
                model_name = "model C",
                manufacturer_name = "manufacturer C",
                max_parallel_charging = 3,
                connectors = new List<Tuple<ConnectorType, int>>(),
            };
            ChargingColumnType chargingColumnType22 = new ChargingColumnType
            {
                id = 2,
                model_name = "model D",
                manufacturer_name = "manufacturer D",
                max_parallel_charging = 3,
                connectors = new List<Tuple<ConnectorType, int>>(),
            };
            int bk_id12 = _chargingColumnTypeDao.Create(chargingColumnType12.model_name, chargingColumnType12.manufacturer_name, chargingColumnType12.max_parallel_charging, chargingColumnType12.connectors);
            int bk_id22 = _chargingColumnTypeDao.Create(chargingColumnType22.model_name, chargingColumnType22.manufacturer_name, chargingColumnType22.max_parallel_charging, chargingColumnType22.connectors);

            // Check if GetById returns the correct chargingColumnType
            Assert.AreEqual(chargingColumnType11.manufacturer_name, _chargingColumnTypeDao.GetById(bk_id11).manufacturer_name);

            Assert.AreEqual(chargingColumnType21.manufacturer_name, _chargingColumnTypeDao.GetById(bk_id21).manufacturer_name);

            Assert.AreEqual(chargingColumnType12.manufacturer_name, _chargingColumnTypeDao.GetById(bk_id12).manufacturer_name);

            Assert.AreEqual(chargingColumnType22.manufacturer_name, _chargingColumnTypeDao.GetById(bk_id22).manufacturer_name);

        }

        
        /// <summary>
        /// Tests if chargingColumnTypes added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewChargingColumnTypeToCache()
        {

            // Check if initial amount of chargingColumnTypes is 0
            List<ChargingColumnType> chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();
            List<ChargingColumnType> chargingColumnTypeList2 = _chargingColumnTypeDao.GetAll();
            Assert.AreEqual(0, chargingColumnTypeList1.Count);
            Assert.AreEqual(0, chargingColumnTypeList2.Count);

            // Create new chargingColumnType in List1 and add it with Create
            _chargingColumnTypeDao.Create(
                "model A",
                "manufacturer A",
                3,
                new List<Tuple<ConnectorType, int>>()
                );

            // Check if there is 1 ChargingColumnType in chargingColumnTypeList1 and still 0 in chargingColumnTypeList2
            chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();
            Assert.AreEqual(1, chargingColumnTypeList1.Count);


            // Create new chargingColumnType in List1 and add it with Create
            _chargingColumnTypeDao.Create(
                "model B",
                "manufacturer B",
                3,
                new List<Tuple<ConnectorType, int>>()
                );


            // Check if there are 2 chargingColumnTypes in chargingColumnTypeList1
            chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();
            Assert.AreEqual(2, chargingColumnTypeList1.Count);
        }
        
        
        
        /// <summary>
        /// Tests if chargingColumnTypes added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewChargingColumnTypeFromCache()
        {

            // Check if initial amount of chargingColumnTypes is 0
            List<ChargingColumnType> chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();
            Assert.AreEqual(0, chargingColumnTypeList1.Count);

            // Create new chargingColumnType in List1 and add it with Create
            int bk_id11 = _chargingColumnTypeDao.Create(
                "model A",
                "manufacturer A",
                3,
                new List<Tuple<ConnectorType, int>>()
                );

            // Create new chargingColumnType in List1 and add it with Create
            int bk_id12 = _chargingColumnTypeDao.Create(
                "model B",
                "manufacturer B",
                3,
                new List<Tuple<ConnectorType, int>>()
                );


            // Check if there is 2 chargingColumnTypes in chargingColumnTypeList1
            chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();
            Assert.AreEqual(2, chargingColumnTypeList1.Count);

            // Delete the first chargingColumnType from chargingColumnTypeList1
            _chargingColumnTypeDao.Delete(bk_id11);

            // Check if there is 1 chargingColumnType in chargingColumnTypeList1
            chargingColumnTypeList1 = _chargingColumnTypeDao.GetAll();
            Assert.AreEqual(1, chargingColumnTypeList1.Count);

            // Check if the second ChargingColumnType really is the correct one left
            Assert.AreEqual("model B", _chargingColumnTypeDao.GetById(bk_id12).model_name);
        }
    }
}