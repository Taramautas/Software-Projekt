using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestChargingZoneDaoImpl
    {
        private ChargingZoneDaoImpl _chargingZoneDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _chargingZoneDao = new ChargingZoneDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditChargingZoneChagesObjectInCache()
        {
            // Create new IDs for Dao Instances
            int list_id1 = ChargingZoneDaoImpl.CreateNewDaoId();

            // Initialize Daos with IDs
            List<ChargingZone> chargingZoneList1 = _chargingZoneDao.GetAll(list_id1);

            // Create new ChargingZones
            int chargingZone_id11 = _chargingZoneDao.Create(new ChargingZoneType(), new ChargingZone { name = "Alpha" }, new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(), list_id1);

            // Edit ChargingZone in Dao1 with GetById
            Assert.AreEqual("Alpha", _chargingZoneDao.GetById(chargingZone_id11, list_id1).charging_zone.name);
            ChargingZone chargingZone11 = _chargingZoneDao.GetById(chargingZone_id11, list_id1);
            chargingZone11.charging_zone.name = "Beta";

            // check if the chargingZone got changed in the cache
            chargingZoneList1 = _chargingZoneDao.GetAll(list_id1);
            Assert.AreEqual("Beta", _chargingZoneDao.GetById(chargingZone_id11, list_id1).charging_zone.name);


        }
        /*
        [Test]
        public void TestGetByIdReturnsCorrectChargingZone()
        {
            // Create new IDs for Dao Instances
            int id1 = ChargingZoneDaoImpl.CreateNewDaoId();
            int id2 = ChargingZoneDaoImpl.CreateNewDaoId();
            // Initialize Daos with IDs
            _chargingZoneDao.GetAll(id1);
            _chargingZoneDao.GetAll(id2);


            // Create new chargingZones in List1 and add it with Create
            ChargingZone chargingZone11 = new ChargingZone
            {
                id = 1,
                charging_column_type_id = new ChargingZoneType(),
                charging_zone = new ChargingZone { name = "Alpha" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            ChargingZone chargingZone21 = new ChargingZone
            {
                id = 2,
                charging_column_type_id = new ChargingZoneType(),
                charging_zone = new ChargingZone { name = "Beta" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            int bk_id11 = _chargingZoneDao.Create(chargingZone11.charging_column_type_id, chargingZone11.charging_zone, chargingZone11.list, id1);
            int bk_id21 = _chargingZoneDao.Create(chargingZone21.charging_column_type_id, chargingZone21.charging_zone, chargingZone21.list, id1);

            // Create new chargingZones in List2 and add it with Create
            ChargingZone chargingZone12 = new ChargingZone
            {
                id = 1,
                charging_column_type_id = new ChargingZoneType(),
                charging_zone = new ChargingZone { name = "Gamma" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            ChargingZone chargingZone22 = new ChargingZone
            {
                id = 2,
                charging_column_type_id = new ChargingZoneType(),
                charging_zone = new ChargingZone { name = "Delta" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            int bk_id12 = _chargingZoneDao.Create(chargingZone12.charging_column_type_id, chargingZone12.charging_zone, chargingZone12.list, id2);
            int bk_id22 = _chargingZoneDao.Create(chargingZone22.charging_column_type_id, chargingZone22.charging_zone, chargingZone22.list, id2);

            // Check if GetById returns the correct chargingZone
            Assert.AreEqual(chargingZone11.charging_zone.name, _chargingZoneDao.GetById(bk_id11, id1).charging_zone.name);

            Assert.AreEqual(chargingZone21.charging_zone.name, _chargingZoneDao.GetById(bk_id21, id1).charging_zone.name);

            Assert.AreEqual(chargingZone12.charging_zone.name, _chargingZoneDao.GetById(bk_id12, id2).charging_zone.name);

            Assert.AreEqual(chargingZone22.charging_zone.name, _chargingZoneDao.GetById(bk_id22, id2).charging_zone.name);

        }

        
        /// <summary>
        /// Tests if chargingZones added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewChargingZoneToCache()
        {
            // Create new IDs for Dao Instances
            int id1 = ChargingZoneDaoImpl.CreateNewDaoId();
            int id2 = ChargingZoneDaoImpl.CreateNewDaoId();

            // Check if initial amount of chargingZones is 0
            List<ChargingZone> chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            List<ChargingZone> chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(0, chargingZoneList1.Count);
            Assert.AreEqual(0, chargingZoneList2.Count);

            // Create new chargingZone in List1 and add it with Create
            _chargingZoneDao.Create(
                new ChargingZoneType(),
                new ChargingZone { name = "Alpha" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1
                );

            // Check if there is 1 ChargingZone in chargingZoneList1 and still 0 in chargingZoneList2
            chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(1, chargingZoneList1.Count);
            Assert.AreEqual(0, chargingZoneList2.Count);


            // Check again with 2 in chargingZoneList1 and 1 in chargingZoneList2
            chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(1, chargingZoneList1.Count);
            Assert.AreEqual(0, chargingZoneList2.Count);

            // Create new chargingZone in List1 and add it with Create
            _chargingZoneDao.Create(
                new ChargingZoneType(),
                new ChargingZone { name = "Beta" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1);

            // Create new chargingZone in List2 and add it with Create
            _chargingZoneDao.Create(
                new ChargingZoneType(),
                new ChargingZone { name = "Gamma" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id2);

            // Check if there is 2 chargingZones in chargingZoneList1 and 1 in chargingZoneList2
            chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(2, chargingZoneList1.Count);
            Assert.AreEqual(1, chargingZoneList2.Count);
        }
        
        
        
        /// <summary>
        /// Tests if chargingZones added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewChargingZoneFromCache()
        {
            // Create new IDs for Dao Instances
            int id1 = ChargingZoneDaoImpl.CreateNewDaoId();
            int id2 = ChargingZoneDaoImpl.CreateNewDaoId();

            // Check if initial amount of chargingZones is 0
            List<ChargingZone> chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            List<ChargingZone> chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(0, chargingZoneList1.Count);
            Assert.AreEqual(0, chargingZoneList2.Count);

            // Create new chargingZone in List1 and add it with Create
            int bk_id11 = _chargingZoneDao.Create(
                new ChargingZoneType(),
                new ChargingZone { name = "Alpha" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1);

            // Create new chargingZone in List1 and add it with Create
            int bk_id12 = _chargingZoneDao.Create(
                new ChargingZoneType(),
                new ChargingZone { name = "Beta" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1);

            // Create new chargingZone in List2 and add it with Create
            _chargingZoneDao.Create(
                new ChargingZoneType(),
                new ChargingZone { name = "Gamma" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id2);

            // Check if there is 2 chargingZones in chargingZoneList1 and 1 in chargingZoneList2
            chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(2, chargingZoneList1.Count);
            Assert.AreEqual(1, chargingZoneList2.Count);

            // Delete the first chargingZone from chargingZoneList1
            _chargingZoneDao.Delete(bk_id11, id1);

            // Check if there is 1 chargingZone in chargingZoneList1 and 1 in chargingZoneList2
            chargingZoneList1 = _chargingZoneDao.GetAll(id1);
            chargingZoneList2 = _chargingZoneDao.GetAll(id2);
            Assert.AreEqual(1, chargingZoneList1.Count);
            Assert.AreEqual(1, chargingZoneList2.Count);

            // Check if the second ChargingZone really is the correct one left
            Assert.AreEqual("Beta", _chargingZoneDao.GetById(bk_id12, id1).charging_zone.name);
        }
        */
    }
}