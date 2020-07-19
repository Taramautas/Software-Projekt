using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class TestLocationDaoImpl
    {
        private LocationDaoImpl _locationDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _locationDao = new LocationDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditLocationChagesObjectInCache()
        {
            // Create new IDs for Dao Instances
            int list_id1 = LocationDaoImpl.CreateNewDaoId();

            // Initialize Daos with IDs
            List<Location> locationList1 = _locationDao.GetAll(list_id1);

            // Create new Locations
            int location_id11 = _locationDao.Create("Alpha", "12345", "adress A", list_id1);

            // Edit Location in Dao1 with GetById
            Assert.AreEqual("Alpha", _locationDao.GetById(location_id11, list_id1).city);
            Location location11 = _locationDao.GetById(location_id11, list_id1);
            location11.city = "Beta";

            // check if the location got changed in the cache
            locationList1 = _locationDao.GetAll(list_id1);
            Assert.AreEqual("Beta", _locationDao.GetById(location_id11, list_id1).city);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectLocation()
        {
            // Create new IDs for Dao Instances
            int id1 = LocationDaoImpl.CreateNewDaoId();
            int id2 = LocationDaoImpl.CreateNewDaoId();
            // Initialize Daos with IDs
            _locationDao.GetAll(id1);
            _locationDao.GetAll(id2);


            // Create new locations in List1 and add it with Create
            Location location11 = new Location
            {
                id = 1,
                city = "Alpha",
                post_code = "12345",
                address = "adress A",
            };
            Location location21 = new Location
            {
                id = 2,
                city = "Beta",
                post_code = "12345",
                address = "adress A",
            };
            int bk_id11 = _locationDao.Create(location11.city, location11.post_code, location11.address, id1);
            int bk_id21 = _locationDao.Create(location21.city, location21.post_code, location21.address, id1);

            // Create new locations in List2 and add it with Create
            Location location12 = new Location
            {
                id = 1,
                city = "Gamma",
                post_code = "12345",
                address = "adress A",
            };
            Location location22 = new Location
            {
                id = 2,
                city = "Delta",
                post_code = "12345",
                address = "adress A",
            };
            int bk_id12 = _locationDao.Create(location12.city, location12.post_code, location12.address, id2);
            int bk_id22 = _locationDao.Create(location22.city, location22.post_code, location22.address, id2);

            // Check if GetById returns the correct location
            Assert.AreEqual(location11.city, _locationDao.GetById(bk_id11, id1).city);

            Assert.AreEqual(location21.city, _locationDao.GetById(bk_id21, id1).city);

            Assert.AreEqual(location12.city, _locationDao.GetById(bk_id12, id2).city);

            Assert.AreEqual(location22.city, _locationDao.GetById(bk_id22, id2).city);

        }

        
        /// <summary>
        /// Tests if locations added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewLocationToCache()
        {
            // Create new IDs for Dao Instances
            int id1 = LocationDaoImpl.CreateNewDaoId();
            int id2 = LocationDaoImpl.CreateNewDaoId();

            // Check if initial amount of locations is 0
            List<Location> locationList1 = _locationDao.GetAll(id1);
            List<Location> locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(0, locationList1.Count);
            Assert.AreEqual(0, locationList2.Count);

            // Create new location in List1 and add it with Create
            _locationDao.Create(
                "Alpha",
                "12345",
                "adress A",
                id1
                );

            // Check if there is 1 Location in locationList1 and still 0 in locationList2
            locationList1 = _locationDao.GetAll(id1);
            locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(1, locationList1.Count);
            Assert.AreEqual(0, locationList2.Count);


            // Check again with 2 in locationList1 and 1 in locationList2
            locationList1 = _locationDao.GetAll(id1);
            locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(1, locationList1.Count);
            Assert.AreEqual(0, locationList2.Count);

            // Create new location in List1 and add it with Create
            _locationDao.Create(
                "Beta",
                "12345",
                "adress A",
                id1);

            // Create new location in List2 and add it with Create
            _locationDao.Create(
                "Gamma",
                "12345",
                "adress A",
                id2);

            // Check if there is 2 locations in locationList1 and 1 in locationList2
            locationList1 = _locationDao.GetAll(id1);
            locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(2, locationList1.Count);
            Assert.AreEqual(1, locationList2.Count);
        }
        
        
        
        /// <summary>
        /// Tests if locations added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewLocationFromCache()
        {
            // Create new IDs for Dao Instances
            int id1 = LocationDaoImpl.CreateNewDaoId();
            int id2 = LocationDaoImpl.CreateNewDaoId();

            // Check if initial amount of locations is 0
            List<Location> locationList1 = _locationDao.GetAll(id1);
            List<Location> locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(0, locationList1.Count);
            Assert.AreEqual(0, locationList2.Count);

            // Create new location in List1 and add it with Create
            int bk_id11 = _locationDao.Create(
                "Alpha",
                "12345",
                "adress A",
                id1);

            // Create new location in List1 and add it with Create
            int bk_id12 = _locationDao.Create(
                "Beta",
                "12345",
                "adress A",
                id1);

            // Create new location in List2 and add it with Create
            _locationDao.Create(
                "Gamma",
                "12345",
                "adress A",
                id2);

            // Check if there is 2 locations in locationList1 and 1 in locationList2
            locationList1 = _locationDao.GetAll(id1);
            locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(2, locationList1.Count);
            Assert.AreEqual(1, locationList2.Count);

            // Delete the first location from locationList1
            _locationDao.Delete(bk_id11, id1);

            // Check if there is 1 location in locationList1 and 1 in locationList2
            locationList1 = _locationDao.GetAll(id1);
            locationList2 = _locationDao.GetAll(id2);
            Assert.AreEqual(1, locationList1.Count);
            Assert.AreEqual(1, locationList2.Count);

            // Check if the second Location really is the correct one left
            Assert.AreEqual("Beta", _locationDao.GetById(bk_id12, id1).city);
        }
    }
}