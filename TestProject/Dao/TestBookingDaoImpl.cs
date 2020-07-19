using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace UnitTest.Dao
{
    public class Tests
    {
        private BookingDaoImpl _bookingDao;
        private UserDaoImpl _userDao;

        // Frage: Kann man die Assert.Equal Methode overriden?

        [SetUp]
        public void Setup()
        {
            _bookingDao = new BookingDaoImpl(new MemoryCache(new MemoryCacheOptions()));
            _userDao = new UserDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestEditBookingChagesObjectInCache()
        {
            // Create new IDs for Dao Instances
            int list_id1 = BookingDaoImpl.CreateNewDaoId();

            // Initialize Daos with IDs
            List<Booking> bookingList1 = _bookingDao.GetAll(list_id1);

            // Create new Bookings
            int booking_id11 = _bookingDao.Create(30, 80, new DateTime(2020, 7, 4, 12, 0, 0), new DateTime(2020, 7, 4, 15, 0, 0), new Vehicle(), new User(), new Location(), list_id1);
            
            // Edit Booking in Dao1 with GetById
            Booking booking11 = _bookingDao.GetById(booking_id11, list_id1);
            booking11.start_state_of_charge = 20;

            // check if the booking got changed in the cache
            bookingList1 = _bookingDao.GetAll(list_id1);
            Assert.AreEqual(bookingList1.Find(x => x.id == booking_id11).start_state_of_charge, 20);
            Booking booking11_test = _bookingDao.GetById(booking_id11, list_id1);
            Assert.AreEqual(booking11_test.start_state_of_charge, 20);


        }
        
        [Test]
        public void TestGetByIdReturnsCorrectBooking()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewDaoId();
            int id2 = BookingDaoImpl.CreateNewDaoId();
            // Initialize Daos with IDs
            _bookingDao.GetAll(id1);
            _bookingDao.GetAll(id2);


            // Create new bookings in List1 and add it with Create
            Booking booking11 = new Booking
            {
                id = 1,
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 4, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 4, 15, 0, 0),
            };
            Booking booking21 = new Booking
            {
                id = 2,
                start_state_of_charge = 40,
                target_state_of_charge = 90,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
            };
            int bk_id11 = _bookingDao.Create(booking11.start_state_of_charge, booking11.target_state_of_charge, booking11.start_time, booking11.end_time, new Vehicle(), new User(), new Location(), id1);
            int bk_id21 = _bookingDao.Create(booking21.start_state_of_charge, booking21.target_state_of_charge, booking21.start_time, booking21.end_time, new Vehicle(), new User(), new Location(), id1);

            // Create new bookings in List2 and add it with Create
            Booking booking12 = new Booking
            {
                id = 1,
                start_state_of_charge = 25,
                target_state_of_charge = 70,
                start_time = new System.DateTime(2020, 7, 4, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 4, 15, 0, 0),
            };
            Booking booking22 = new Booking
            {
                id = 2,
                start_state_of_charge = 10,
                target_state_of_charge = 85,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
            };
            int bk_id12 = _bookingDao.Create(booking12.start_state_of_charge, booking12.target_state_of_charge, booking12.start_time, booking12.end_time, new Vehicle(), new User(), new Location(), id2);
            int bk_id22 = _bookingDao.Create(booking22.start_state_of_charge, booking22.target_state_of_charge, booking22.start_time, booking22.end_time, new Vehicle(), new User(), new Location(), id2);

            // Check if GetById returns the correct booking
            Assert.AreEqual(booking11.start_state_of_charge, _bookingDao.GetById(bk_id11, id1).start_state_of_charge);
            Assert.AreEqual(booking11.target_state_of_charge, _bookingDao.GetById(bk_id11, id1).target_state_of_charge);

            Assert.AreEqual(booking21.start_state_of_charge, _bookingDao.GetById(bk_id21, id1).start_state_of_charge);
            Assert.AreEqual(booking21.target_state_of_charge, _bookingDao.GetById(bk_id21, id1).target_state_of_charge);

            Assert.AreEqual(booking12.start_state_of_charge, _bookingDao.GetById(bk_id12, id2).start_state_of_charge);
            Assert.AreEqual(booking12.target_state_of_charge, _bookingDao.GetById(bk_id12, id2).target_state_of_charge);

            Assert.AreEqual(booking22.start_state_of_charge, _bookingDao.GetById(bk_id22, id2).start_state_of_charge);
            Assert.AreEqual(booking22.target_state_of_charge, _bookingDao.GetById(bk_id22, id2).target_state_of_charge);

        }


        /// <summary>
        /// Tests if bookings added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewBookingToCache()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewDaoId();
            int id2 = BookingDaoImpl.CreateNewDaoId();

            // Check if initial amount of bookings is 0
            List<Booking> bookingList1 = _bookingDao.GetAll(id1);
            List<Booking> bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(0, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Create new booking in List1 and add it with Create
            _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 4, 12, 0, 0),
                new System.DateTime(2020, 7, 4, 15, 0, 0),
                new Vehicle(),
                new User(),
                new Location(),
                id1
                );

            // Check if there is 1 Booking in bookingList1 and still 0 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(1, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);


            // Check again with 2 in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(1, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Create new booking in List1 and add it with Create
            _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                new User(),
                new Location(),
                id1);

            // Create new booking in List2 and add it with Create
            _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                new User(),
                new Location(),
                id2);

            // Check if there is 2 bookings in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(2, bookingList1.Count);
            Assert.AreEqual(1, bookingList2.Count);
        }

        /// <summary>
        /// Tests if bookings added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateCreatesNewBookingToCache()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewDaoId();
            int id2 = BookingDaoImpl.CreateNewDaoId();

            // Check if initial amount of bookings is 0
            List<Booking> bookingList1 = _bookingDao.GetAll(id1);
            List<Booking> bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(0, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Create new booking in List1 and add it with Create
            Vehicle vehicle = new Vehicle { model_name = "Tesla" };
            int bookingId11 = _bookingDao.Create(30, 100, new System.DateTime(2020, 7, 4, 12, 0, 0), new System.DateTime(2020, 7, 4, 15, 0, 0), vehicle, new User(), new Location(), id1);

            // Check if there is 1 Booking in bookingList1 and still 0 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(1, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Check if we find the correct booking with the returned id
            Assert.AreEqual(_bookingDao.GetById(bookingId11, id1).id, bookingId11);
            Assert.AreEqual(_bookingDao.GetById(bookingId11, id1).start_state_of_charge, 30);
            Assert.AreEqual(_bookingDao.GetById(bookingId11, id1).target_state_of_charge, 100);
            Assert.AreEqual(_bookingDao.GetById(bookingId11, id1).start_time, new System.DateTime(2020, 7, 4, 12, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId11, id1).end_time, new System.DateTime(2020, 7, 4, 15, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId11, id1).vehicle, vehicle);

            

            // Create new booking in List1 and add it with Create
            int bookingId21 = _bookingDao.Create(30, 100, new System.DateTime(2020, 7, 5, 12, 0, 0), new System.DateTime(2020, 7, 5, 15, 0, 0), vehicle, new User(), new Location(), id1);
            Console.WriteLine("bookingId21: " + bookingId21);

            // Create new booking in List2 and add it with Create
            int bookingId12 = _bookingDao.Create(30, 80, new System.DateTime(2020, 7, 5, 12, 0, 0), new System.DateTime(2020, 7, 5, 15, 0, 0), vehicle, new User(), new Location(), id2);

            // Check if there is 2 bookings in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(2, bookingList1.Count);
            Assert.AreEqual(1, bookingList2.Count);

            // Check again if we find the correct booking with the returned id
            Assert.AreEqual(_bookingDao.GetById(bookingId21, id1).id, bookingId21);
            Assert.AreEqual(_bookingDao.GetById(bookingId21, id1).start_state_of_charge, 30);
            Assert.AreEqual(_bookingDao.GetById(bookingId21, id1).target_state_of_charge, 100);
            Assert.AreEqual(_bookingDao.GetById(bookingId21, id1).start_time, new System.DateTime(2020, 7, 5, 12, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId21, id1).end_time, new System.DateTime(2020, 7, 5, 15, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId21, id1).vehicle, vehicle);



            Assert.AreEqual(_bookingDao.GetById(bookingId12, id2).id, bookingId12);
            Assert.AreEqual(_bookingDao.GetById(bookingId12, id2).start_state_of_charge, 30);
            Assert.AreEqual(_bookingDao.GetById(bookingId12, id2).target_state_of_charge, 80);
            Assert.AreEqual(_bookingDao.GetById(bookingId12, id2).start_time, new System.DateTime(2020, 7, 5, 12, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId12, id2).end_time, new System.DateTime(2020, 7, 5, 15, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId12, id2).vehicle, vehicle);

  

            // Test Create with new DaoId
            int id3 = BookingDaoImpl.CreateNewDaoId();
            int bookingId13 = _bookingDao.Create(30, 100, new System.DateTime(2020, 7, 5, 12, 0, 0), new System.DateTime(2020, 7, 5, 15, 0, 0), vehicle, new User(), new Location(), id3);

            // Check if there is 1 bookings in bookingList3
            List<Booking> bookingList3 = _bookingDao.GetAll(id3);
            Assert.AreEqual(1, bookingList3.Count);

            // Check again if we find the correct booking with the returned id
            Assert.AreEqual(_bookingDao.GetById(bookingId13, id3).id, bookingId13);
            Assert.AreEqual(_bookingDao.GetById(bookingId13, id3).start_state_of_charge, 30);
            Assert.AreEqual(_bookingDao.GetById(bookingId13, id3).target_state_of_charge, 100);
            Assert.AreEqual(_bookingDao.GetById(bookingId13, id3).start_time, new System.DateTime(2020, 7, 5, 12, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId13, id3).end_time, new System.DateTime(2020, 7, 5, 15, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bookingId13, id3).vehicle, vehicle);

        }

        /// <summary>
        /// Tests if bookings added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestDeleteRemovesNewBookingFromCache()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewDaoId();
            int id2 = BookingDaoImpl.CreateNewDaoId();

            // Check if initial amount of bookings is 0
            List<Booking> bookingList1 = _bookingDao.GetAll(id1);
            List<Booking> bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(0, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Create new booking in List1 and add it with Create
            int bk_id11 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 4, 12, 0, 0),
                new System.DateTime(2020, 7, 4, 15, 0, 0),
                new Vehicle(),
                new User(),
                new Location(),
                id1);

            // Create new booking in List1 and add it with Create
            int bk_id12 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                new User(),
                new Location(),
                id1);

            // Create new booking in List2 and add it with Create
            _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                new User(),
                new Location(),
                id2);

            // Check if there is 2 bookings in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(2, bookingList1.Count);
            Assert.AreEqual(1, bookingList2.Count);

            // Delete the first booking from bookingList1
            _bookingDao.Delete(bk_id11, id1);

            // Check if there is 1 booking in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(1, bookingList1.Count);
            Assert.AreEqual(1, bookingList2.Count);

            // Check if the second Booking really is the correct one left
            Assert.AreEqual(_bookingDao.GetById(bk_id12, id1).start_state_of_charge, 30);
            Assert.AreEqual(_bookingDao.GetById(bk_id12, id1).target_state_of_charge, 100);
            Assert.AreEqual(_bookingDao.GetById(bk_id12, id1).start_time, new System.DateTime(2020, 7, 5, 12, 0, 0));
            Assert.AreEqual(_bookingDao.GetById(bk_id12, id1).end_time, new System.DateTime(2020, 7, 5, 15, 0, 0));
        }

        [Test]
        public void TestGetOpenBookingsByUserId()
        {
            // Create some users
            _userDao.GetAll();
            int user_id1 = _userDao.Create("user1", "user1@user1.de", "user1", Role.Employee);
            int user_id2 = _userDao.Create("user2", "user1@user2.de", "user2", Role.Employee);

            // Create some bookings
            int bk_id1 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                _userDao.GetById(user_id1),
                new Location(),
                0
            );

            int bk_id2 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                _userDao.GetById(user_id1),
                new Location(),
                0
            );

            int bk_id3 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                _userDao.GetById(user_id2),
                new Location(),
                0
            );

            Assert.AreEqual(2, _bookingDao.GetOpenBookingsByUserId(user_id1).Count);
            Assert.AreEqual(1, _bookingDao.GetOpenBookingsByUserId(user_id2).Count);
            _bookingDao.GetById(bk_id1, 0).Accept();
            Assert.AreEqual(1, _bookingDao.GetOpenBookingsByUserId(user_id1).Count);
            Assert.AreEqual(1, _bookingDao.GetOpenBookingsByUserId(user_id2).Count);
        }

        [Test]
        public void TestGetAcceptedBookingsByUserId()
        {
            // Create some users
            _userDao.GetAll();
            int user_id1 = _userDao.Create("user1", "user1@user1.de", "user1", Role.Employee);
            int user_id2 = _userDao.Create("user2", "user1@user2.de", "user2", Role.Employee);

            // Create some bookings
            int bk_id1 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                _userDao.GetById(user_id1),
                new Location(),
                0
            );

            int bk_id2 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                _userDao.GetById(user_id1),
                new Location(),
                0
            );

            int bk_id3 = _bookingDao.Create(
                30,
                100,
                new System.DateTime(2020, 7, 5, 12, 0, 0),
                new System.DateTime(2020, 7, 5, 15, 0, 0),
                new Vehicle(),
                _userDao.GetById(user_id2),
                new Location(),
                0
            );

            Assert.AreEqual(0, _bookingDao.GetAcceptedBookingsByUserId(user_id1).Count);
            Assert.AreEqual(0, _bookingDao.GetAcceptedBookingsByUserId(user_id2).Count);
            _bookingDao.GetById(bk_id1, 0).Accept();
            Assert.AreEqual(1, _bookingDao.GetAcceptedBookingsByUserId(user_id1).Count);
            Assert.AreEqual(0, _bookingDao.GetAcceptedBookingsByUserId(user_id2).Count);
        }
    }
}