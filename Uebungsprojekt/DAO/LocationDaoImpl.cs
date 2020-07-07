using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using static Uebungsprojekt.Models.Location;

namespace Uebungsprojekt.DAO
{
    public class LocationDaoImpl : LocationDao
    {
        private IMemoryCache _cache;
        private static int DaoId = 0;

        public LocationDaoImpl(IMemoryCache memoryCache)
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
        /// Creates and adds a location to the Locationlist if there is one, else it creates a new List and adds the location
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the id of the added Location</returns>
        public int Create(string _City, string _Post_code, string _Adress, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateLocationIds", out int ids))
            {
                ++ids;
                _cache.Set(DaoId + "CreateLocationIds", ids);
                _cache.TryGetValue(DaoId + "CreateLocation", out List<Location> createdLocations);
                Location newLocation = new Location
                {
                    id = ids,
                    city = _City,
                    post_code = _Post_code,
                    address = _Adress,
                };
                createdLocations.Add(newLocation);
                return ids;
            }

            else
            {
                List<Location> createdLocations = new List<Location>();
                ids = 0;
                Location newLocation = new Location
                {
                    id = ids,
                    city = _City,
                    post_code = _Post_code,
                    address = _Adress,
                };
                createdLocations.Add(newLocation);
                _cache.Set(DaoId + "CreateLocation", createdLocations);
                _cache.Set(DaoId + "CreateLocationIds", ids);
                return ids;
            }
        } // Frage: Soll Create überhaupt in der Lage sein eine neue DaoListe anzulegen falls die id nicht vorhanden ist?

        /// <summary>
        /// Adds a location to the Locationlist if there is one, else it creates a new List and adds the location
        /// </summary>
        /// <param name="location">Location that is to be added</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the added Location</returns>
        public Location Create(Location location, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateLocation", out List<Location> createdLocations))
            {
                createdLocations.Add(location);
                return location;
            }
            else
            {
                createdLocations = new List<Location> { location };
                _cache.Set(DaoId + "CreateLocation", createdLocations);
                return location;
            }
        } // Frage: Soll das so überhaupt bestehen bleiben?

        /// <summary>
        /// Delets the Location with specified Id
        /// </summary>
        /// <param name="_Id">Location Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateLocation", out List<Location> createdLocations))
            {
                if (GetById(_Id, DaoId) == null)
                {
                    return false;
                }
                createdLocations.Remove(GetById(_Id, DaoId));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Locations in Cache with specified id if there is one, else it creates a new List and returns it
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>List of Locations</returns>
        public List<Location> GetAll(int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateLocation", out List<Location> createdLocations))
            {
                return createdLocations;
            }
            else
            {
                int ids = 0;
                createdLocations = new List<Location>();
                _cache.Set(DaoId + "CreateLocation", createdLocations);
                _cache.Set(DaoId + "CreateLocationIds", ids);
                return createdLocations;
            }
        }


        /// <summary>
        /// Finds a Location with specified ID and returns it
        /// </summary>
        /// <param name="_Id">Location Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>Location with specified Id on success and null on failure</returns>
        public Location GetById(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateLocation", out List<Location> createdLocations))
            {

                return createdLocations.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
