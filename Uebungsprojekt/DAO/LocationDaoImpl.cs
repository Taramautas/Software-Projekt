using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class LocationDaoImpl : LocationDao
    {
        private IMemoryCache _cache;

        public LocationDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a location to the Locationlist if there is one, else it creates a new List and adds the location
        /// </summary>
        /// <param name="location">Location that is to be added</param>
        /// <returns>the added Location</returns>
        public Location Create(Location location)
        {
            if (_cache.TryGetValue("CreateLocation", out List<Location> createdLocations))
            {
                createdLocations.Add(location);
                return location;
            }
            else
            {
                createdLocations = new List<Location> { location };
                _cache.Set("CreateLocation", createdLocations);
                return location;
            }
        }

        /// <summary>
        /// Delets the Location with specified Id
        /// </summary>
        /// <param name="_Id">Location Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateLocation", out List<Location> createdLocations))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdLocations.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Locations in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of Locations</returns>
        public List<Location> GetAll()
        {
            if (_cache.TryGetValue("CreateLocation", out List<Location> createdLocations))
            {
                return createdLocations;
            }
            else
            {
                createdLocations = new List<Location>();
                _cache.Set("CreateLocation", createdLocations);
                return createdLocations;
            }
        }

        /// <summary>
        /// Finds a Location with specified ID and returns it
        /// </summary>
        /// <param name="_Id">Location Id</param>
        /// <returns>Location with specified Id on success and null on failure</returns>
        public Location GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateLocation", out List<Location> createdLocations))
            {

                return createdLocations.Find(x => x.Id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
