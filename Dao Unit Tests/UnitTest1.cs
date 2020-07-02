using NUnit.Framework;
using Uebungsprojekt.DAO;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Dao_Unit_Tests
{
    public class Tests
    {
        private BookingDaoImpl _bookingDao;

        [SetUp]
        public void Setup()
        {
            _bookingDao = new BookingDaoImpl(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void TestGetByIdReturnsCorrectBooking()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewId();
            int id2 = BookingDaoImpl.CreateNewId();
            // Initialize Daos with IDs
            _bookingDao.GetAll(id1);
            _bookingDao.GetAll(id2);


            // Create new bookings in List1 and add it with Create
            Booking booking11 = new Booking
            {
                Id = 1,
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 4, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 4, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            };
            Booking booking21 = new Booking
            {
                Id = 2,
                start_state_of_charge = 40,
                target_state_of_charge = 90,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.Tesla_Supercharger,
            };
            _bookingDao.Create(booking11, id1);
            _bookingDao.Create(booking21, id1);

            // Create new bookings in List2 and add it with Create
            Booking booking12 = new Booking
            {
                Id = 1,
                start_state_of_charge = 25,
                target_state_of_charge = 70,
                start_time = new System.DateTime(2020, 7, 4, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 4, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            };
            Booking booking22 = new Booking
            {
                Id = 2,
                start_state_of_charge = 10,
                target_state_of_charge = 85,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.Tesla_Supercharger,
            };
            _bookingDao.Create(booking12, id2);
            _bookingDao.Create(booking22, id2);

            // Check if GetById returns the correct booking
            Assert.AreEqual(booking11, _bookingDao.GetById(1, id1));
            Assert.AreEqual(booking21, _bookingDao.GetById(2, id1));
            Assert.AreEqual(booking12, _bookingDao.GetById(1, id2));
            Assert.AreEqual(booking22, _bookingDao.GetById(2, id2));

            Console.WriteLine("ID1: " + id1);
            Debug.WriteLine("ID1: " + id1);
            Debug.WriteLine("ID2: " + id2);
        }

        /// <summary>
        /// Tests if bookings added to Dao with Create are added to the Cache with the right Key
        /// </summary>
        [Test]
        public void TestCreateAddsNewBookingToCache()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewId();
            int id2 = BookingDaoImpl.CreateNewId();

            // Check if initial amount of bookings is 0
            List<Booking> bookingList1 = _bookingDao.GetAll(id1);
            List<Booking> bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(0, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Create new booking in List1 and add it with Create
            _bookingDao.Create(new Booking
            {
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 4, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 4, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            }, id1);

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
            _bookingDao.Create(new Booking
            {
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            }, id1);

            // Create new booking in List2 and add it with Create
            _bookingDao.Create(new Booking
            {
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            }, id2);

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
        public void TestDeleteRemovesNewBookingFromCache()
        {
            // Create new IDs for Dao Instances
            int id1 = BookingDaoImpl.CreateNewId();
            int id2 = BookingDaoImpl.CreateNewId();

            // Check if initial amount of bookings is 0
            List<Booking> bookingList1 = _bookingDao.GetAll(id1);
            List<Booking> bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(0, bookingList1.Count);
            Assert.AreEqual(0, bookingList2.Count);

            // Create new booking in List1 and add it with Create
            _bookingDao.Create(new Booking
            {
                Id = 1,
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 4, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 4, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            }, id1);

            // Create new booking in List1 and add it with Create
            Booking booking = new Booking
            {
                Id = 2,
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            };
            _bookingDao.Create(booking, id1);

            // Create new booking in List2 and add it with Create
            _bookingDao.Create(new Booking
            {
                Id = 1,
                start_state_of_charge = 30,
                target_state_of_charge = 100,
                start_time = new System.DateTime(2020, 7, 5, 12, 0, 0),
                end_time = new System.DateTime(2020, 7, 5, 15, 0, 0),
                ConnectorType = Booking.ConnectorTypeEnum.CCS_Combo_2_Plug,
            }, id2);

            // Check if there is 2 bookings in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(2, bookingList1.Count);
            Assert.AreEqual(1, bookingList2.Count);

            // Delete the first booking from bookingList1
            _bookingDao.Delete(1, id1);

            // Check if there is 1 booking in bookingList1 and 1 in bookingList2
            bookingList1 = _bookingDao.GetAll(id1);
            bookingList2 = _bookingDao.GetAll(id2);
            Assert.AreEqual(1, bookingList1.Count);
            Assert.AreEqual(1, bookingList2.Count);

            // Check if the second Booking really is the correct one left
            Assert.AreEqual(_bookingDao.GetById(2, id1), booking);
        }
    }
}