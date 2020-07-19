using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestChargingColumnDaoImpl
    {
        private ChargingColumnDaoImpl _chargingColumnDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _chargingColumnDao = new ChargingColumnDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditChargingColumnChagesObjectInCache()
        {
            // Create new IDs for Dao Instances
            int list_id1 = ChargingColumnDaoImpl.CreateNewDaoId();

            // Initialize Daos with IDs
            List<ChargingColumn> chargingColumnList1 = _chargingColumnDao.GetAll(list_id1);

            // Create new ChargingColumns
            int chargingColumn_id11 = _chargingColumnDao.Create(new ChargingColumnType(), new ChargingZone { name = "Alpha" }, new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(), list_id1);

            // Edit ChargingColumn in Dao1 with GetById
            Assert.AreEqual("Alpha", _chargingColumnDao.GetById(chargingColumn_id11, list_id1).charging_zone.name);
            ChargingColumn chargingColumn11 = _chargingColumnDao.GetById(chargingColumn_id11, list_id1);
            chargingColumn11.charging_zone.name = "Beta";

            // check if the chargingColumn got changed in the cache
            chargingColumnList1 = _chargingColumnDao.GetAll(list_id1);
            Assert.AreEqual("Beta", _chargingColumnDao.GetById(chargingColumn_id11, list_id1).charging_zone.name);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectChargingColumn()
        {
            // Create new IDs for Dao Instances
            int id1 = ChargingColumnDaoImpl.CreateNewDaoId();
            int id2 = ChargingColumnDaoImpl.CreateNewDaoId();
            // Initialize Daos with IDs
            _chargingColumnDao.GetAll(id1);
            _chargingColumnDao.GetAll(id2);


            // Create new chargingColumns in List1 and add it with Create
            ChargingColumn chargingColumn11 = new ChargingColumn
            {
                id = 1,
                charging_column_type_id = new ChargingColumnType(),
                charging_zone = new ChargingZone { name = "Alpha" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            ChargingColumn chargingColumn21 = new ChargingColumn
            {
                id = 2,
                charging_column_type_id = new ChargingColumnType(),
                charging_zone = new ChargingZone { name = "Beta" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            int bk_id11 = _chargingColumnDao.Create(chargingColumn11.charging_column_type_id, chargingColumn11.charging_zone, chargingColumn11.list, id1);
            int bk_id21 = _chargingColumnDao.Create(chargingColumn21.charging_column_type_id, chargingColumn21.charging_zone, chargingColumn21.list, id1);

            // Create new chargingColumns in List2 and add it with Create
            ChargingColumn chargingColumn12 = new ChargingColumn
            {
                id = 1,
                charging_column_type_id = new ChargingColumnType(),
                charging_zone = new ChargingZone { name = "Gamma" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            ChargingColumn chargingColumn22 = new ChargingColumn
            {
                id = 2,
                charging_column_type_id = new ChargingColumnType(),
                charging_zone = new ChargingZone { name = "Delta" },
                list = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
            };
            int bk_id12 = _chargingColumnDao.Create(chargingColumn12.charging_column_type_id, chargingColumn12.charging_zone, chargingColumn12.list, id2);
            int bk_id22 = _chargingColumnDao.Create(chargingColumn22.charging_column_type_id, chargingColumn22.charging_zone, chargingColumn22.list, id2);

            // Check if GetById returns the correct chargingColumn
            Assert.AreEqual(chargingColumn11.charging_zone.name, _chargingColumnDao.GetById(bk_id11, id1).charging_zone.name);

            Assert.AreEqual(chargingColumn21.charging_zone.name, _chargingColumnDao.GetById(bk_id21, id1).charging_zone.name);

            Assert.AreEqual(chargingColumn12.charging_zone.name, _chargingColumnDao.GetById(bk_id12, id2).charging_zone.name);

            Assert.AreEqual(chargingColumn22.charging_zone.name, _chargingColumnDao.GetById(bk_id22, id2).charging_zone.name);

        }

        
        /// <summary>
        /// Tests if chargingColumns added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewChargingColumnToCache()
        {
            // Create new IDs for Dao Instances
            int id1 = ChargingColumnDaoImpl.CreateNewDaoId();
            int id2 = ChargingColumnDaoImpl.CreateNewDaoId();

            // Check if initial amount of chargingColumns is 0
            List<ChargingColumn> chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            List<ChargingColumn> chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(0, chargingColumnList1.Count);
            Assert.AreEqual(0, chargingColumnList2.Count);

            // Create new chargingColumn in List1 and add it with Create
            _chargingColumnDao.Create(
                new ChargingColumnType(),
                new ChargingZone { name = "Alpha" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1
                );

            // Check if there is 1 ChargingColumn in chargingColumnList1 and still 0 in chargingColumnList2
            chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(1, chargingColumnList1.Count);
            Assert.AreEqual(0, chargingColumnList2.Count);


            // Check again with 2 in chargingColumnList1 and 1 in chargingColumnList2
            chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(1, chargingColumnList1.Count);
            Assert.AreEqual(0, chargingColumnList2.Count);

            // Create new chargingColumn in List1 and add it with Create
            _chargingColumnDao.Create(
                new ChargingColumnType(),
                new ChargingZone { name = "Beta" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1);

            // Create new chargingColumn in List2 and add it with Create
            _chargingColumnDao.Create(
                new ChargingColumnType(),
                new ChargingZone { name = "Gamma" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id2);

            // Check if there is 2 chargingColumns in chargingColumnList1 and 1 in chargingColumnList2
            chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(2, chargingColumnList1.Count);
            Assert.AreEqual(1, chargingColumnList2.Count);
        }
        
        
        
        /// <summary>
        /// Tests if chargingColumns added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewChargingColumnFromCache()
        {
            // Create new IDs for Dao Instances
            int id1 = ChargingColumnDaoImpl.CreateNewDaoId();
            int id2 = ChargingColumnDaoImpl.CreateNewDaoId();

            // Check if initial amount of chargingColumns is 0
            List<ChargingColumn> chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            List<ChargingColumn> chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(0, chargingColumnList1.Count);
            Assert.AreEqual(0, chargingColumnList2.Count);

            // Create new chargingColumn in List1 and add it with Create
            int bk_id11 = _chargingColumnDao.Create(
                new ChargingColumnType(),
                new ChargingZone { name = "Alpha" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1);

            // Create new chargingColumn in List1 and add it with Create
            int bk_id12 = _chargingColumnDao.Create(
                new ChargingColumnType(),
                new ChargingZone { name = "Beta" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id1);

            // Create new chargingColumn in List2 and add it with Create
            _chargingColumnDao.Create(
                new ChargingColumnType(),
                new ChargingZone { name = "Gamma" },
                new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>(),
                id2);

            // Check if there is 2 chargingColumns in chargingColumnList1 and 1 in chargingColumnList2
            chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(2, chargingColumnList1.Count);
            Assert.AreEqual(1, chargingColumnList2.Count);

            // Delete the first chargingColumn from chargingColumnList1
            _chargingColumnDao.Delete(bk_id11, id1);

            // Check if there is 1 chargingColumn in chargingColumnList1 and 1 in chargingColumnList2
            chargingColumnList1 = _chargingColumnDao.GetAll(id1);
            chargingColumnList2 = _chargingColumnDao.GetAll(id2);
            Assert.AreEqual(1, chargingColumnList1.Count);
            Assert.AreEqual(1, chargingColumnList2.Count);

            // Check if the second ChargingColumn really is the correct one left
            Assert.AreEqual("Beta", _chargingColumnDao.GetById(bk_id12, id1).charging_zone.name);
        }
    }
}