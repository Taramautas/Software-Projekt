using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class BookingDaoImpl : BookingDao
    {
        private IMemoryCache _cache;
        public static int id = 0;

        public BookingDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Creates new DaoId and returns it.
        /// </summary>
        /// <returns>the created DaoId</returns>
        public static int CreateNewId()
        {
            id++;
            return id;
        }

        /// <summary>
        /// Adds a booking to the Bookinglist if there is one, else it creates a new List and adds the booking
        /// </summary>
        /// <param name="booking">Booking that is to be added</param>
        /// <param name="DaoId">Id of List that's to be used. If DaoId = 0 a new Id will be created</param>
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
        } // Frage: Soll Create überhaupt in der Lage sein eine neue DaoListe anzulegen falls die id nicht vorhanden ist?

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
        /// Returns the List of Bookings in Cache if there is one, else it creates a new List and returns it
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
                createdBookings = new List<Booking>();
                _cache.Set(DaoId + "CreateBooking", createdBookings);
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

                return createdBookings.Find(x => x.Id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
