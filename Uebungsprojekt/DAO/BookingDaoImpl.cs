using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class BookingDaoImpl : BookingDao
    {
        private IMemoryCache _cache;

        public BookingDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a booking to the Bookinglist if there is one, else it creates a new List and adds the booking
        /// </summary>
        /// <param name="booking">Booking that is to be added</param>
        /// <returns>the added Booking</returns>
        public Booking Create(Booking booking)
        {
            if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
            {
                createdBookings.Add(booking);
                return booking;
            }
            else
            {
                createdBookings = new List<Booking> { booking };
                _cache.Set("CreateBooking", createdBookings);
                return booking;
            }
        }

        /// <summary>
        /// Delets the Booking with specified Id
        /// </summary>
        /// <param name="_Id">Booking Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
            {
                if(GetById(_Id) == null)
                {
                    return false;
                }
                createdBookings.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Bookings in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of Bookings</returns>
        public List<Booking> GetAll()
        {
            if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
            {
                return createdBookings;
            }
            else
            {
                createdBookings = new List<Booking>();
                _cache.Set("CreateBooking", createdBookings);
                return createdBookings;
            }
        }

        /// <summary>
        /// Finds a Booking with specified ID and returns it
        /// </summary>
        /// <param name="_Id">Booking Id</param>
        /// <returns>Booking with specified Id on success and null on failure</returns>
        public Booking GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
            {

                return createdBookings.Find(x => x.Id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
