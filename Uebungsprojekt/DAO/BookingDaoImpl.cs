using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using static Uebungsprojekt.Models.Booking;

namespace Uebungsprojekt.DAO
{
    public class BookingDaoImpl : BookingDao
    {
        private IMemoryCache _cache;
        private static int DaoId = 0;

        public BookingDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Creates new DaoId and returns it.
        /// </summary>
        /// <returns>the created DaoId</returns>
        public static int CreateNewDaoId()
        {
            DaoId++;
            return DaoId;
        }

        /// <summary>
        /// Creates and adds a booking to the Bookinglist if there is one, else it creates a new List and adds the booking
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the id of the added Booking</returns>
        public int Create(int _start_state_of_charge, int _target_state_of_charge, DateTime _start_time, DateTime _end_time, Vehicle _vehicle, ConnectorType _connectorType, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateBookingIds", out int ids))
            {
                ++ids;
                _cache.Set(DaoId + "CreateBookingIds", ids);
                _cache.TryGetValue(DaoId + "CreateBooking", out List<Booking> createdBookings);
                Booking newBooking = new Booking
                {
                    id = ids,
                    start_state_of_charge = _start_state_of_charge,
                    target_state_of_charge = _target_state_of_charge,
                    start_time = _start_time,
                    end_time = _end_time,
                    vehicle = _vehicle,
                };
                createdBookings.Add(newBooking);
                return ids;
            }
            
            else
            {
                List<Booking> createdBookings = new List<Booking>();
                ids = 0;
                Booking newBooking = new Booking
                {
                    id = ++ids,
                    start_state_of_charge = _start_state_of_charge,
                    target_state_of_charge = _target_state_of_charge,
                    start_time = _start_time,
                    end_time = _end_time,
                    vehicle = _vehicle,
                };
                createdBookings.Add(newBooking);
                _cache.Set(DaoId + "CreateBooking", createdBookings);
                _cache.Set(DaoId + "CreateBookingIds", ids);
                return ids;
            }
        } // Frage: Soll Create überhaupt in der Lage sein eine neue DaoListe anzulegen falls die id nicht vorhanden ist?

        /// <summary>
        /// Adds a booking to the Bookinglist if there is one, else it creates a new List and adds the booking
        /// </summary>
        /// <param name="booking">Booking that is to be added</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the added Booking</returns>
        public Booking Create(Booking booking, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateBooking", out List<Booking> createdBookings))
            {
                createdBookings.Add(booking);
                return booking;
            }
            else
            {
                createdBookings = new List<Booking> { booking };
                _cache.Set(DaoId + "CreateBooking", createdBookings);
                return booking;
            }
        } // Frage: Soll das so überhaupt bestehen bleiben?

        /// <summary>
        /// Delets the Booking with specified Id
        /// </summary>
        /// <param name="_Id">Booking Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateBooking", out List<Booking> createdBookings))
            {
                if(GetById(_Id, DaoId) == null)
                {
                    return false;
                }
                createdBookings.Remove(GetById(_Id, DaoId));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Bookings in Cache with specified id if there is one, else it creates a new List and returns it
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>List of Bookings</returns>
        public List<Booking> GetAll(int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateBooking", out List<Booking> createdBookings))
            {
                return createdBookings;
            }
            else
            {
                int ids = 0;
                createdBookings = new List<Booking>();
                _cache.Set(DaoId + "CreateBooking", createdBookings);
                _cache.Set(DaoId + "CreateBookingIds", ids);
                return createdBookings;
            }
        }


        /// <summary>
        /// Finds a Booking with specified ID and returns it
        /// </summary>
        /// <param name="_Id">Booking Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>Booking with specified Id on success and null on failure</returns>
        public Booking GetById(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateBooking", out List<Booking> createdBookings))
            {

                return createdBookings.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
