using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using static Uebungsprojekt.Models.ChargingZone;

namespace Uebungsprojekt.DAO
{
    public class ChargingZoneDaoImpl : ChargingZoneDao
    {
        private IMemoryCache _cache;
        private static int DaoId = 0;

        public ChargingZoneDaoImpl(IMemoryCache memoryCache)
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
        /// Creates and adds a chargingZone to the ChargingZonelist if there is one, else it creates a new List and adds the chargingZone
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the id of the added ChargingZone</returns>
        public int Create(int _Overall_performance, Location _location, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingZoneIds", out int ids))
            {
                ++ids;
                _cache.Set(DaoId + "CreateChargingZoneIds", ids);
                _cache.TryGetValue(DaoId + "CreateChargingZone", out List<ChargingZone> createdChargingZones);
                ChargingZone newChargingZone = new ChargingZone
                {
                    id = ids,
                    overall_performance = _Overall_performance,
                    location = _location,
                };
                createdChargingZones.Add(newChargingZone);
                return ids;
            }

            else
            {
                List<ChargingZone> createdChargingZones = new List<ChargingZone>();
                ids = 0;
                ChargingZone newChargingZone = new ChargingZone
                {
                    id = ++ids,
                    overall_performance = _Overall_performance,
                };
                createdChargingZones.Add(newChargingZone);
                _cache.Set(DaoId + "CreateChargingZone", createdChargingZones);
                _cache.Set(DaoId + "CreateChargingZoneIds", ids);
                return ids;
            }
        } 

        
        /// <summary>
        /// Delets the ChargingZone with specified Id
        /// </summary>
        /// <param name="_Id">ChargingZone Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {
                if (GetById(_Id, DaoId) == null)
                {
                    return false;
                }
                createdChargingZones.Remove(GetById(_Id, DaoId));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of ChargingZones in Cache with specified id if there is one, else it creates a new List and returns it
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>List of ChargingZones</returns>
        public List<ChargingZone> GetAll(int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {
                return createdChargingZones;
            }
            else
            {
                int ids = 0;
                createdChargingZones = new List<ChargingZone>();
                _cache.Set(DaoId + "CreateChargingZone", createdChargingZones);
                _cache.Set(DaoId + "CreateChargingZoneIds", ids);
                return createdChargingZones;
            }
        }


        /// <summary>
        /// Finds a ChargingZone with specified ID and returns it
        /// </summary>
        /// <param name="_Id">ChargingZone Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>ChargingZone with specified Id on success and null on failure</returns>
        public ChargingZone GetById(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {

                return createdChargingZones.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
