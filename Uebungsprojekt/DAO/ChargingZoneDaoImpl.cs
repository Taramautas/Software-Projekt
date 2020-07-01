using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class ChargingZoneDaoImpl : ChargingZoneDao
    {
        private IMemoryCache _cache;

        public ChargingZoneDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a chargingZone to the ChargingZonelist if there is one, else it creates a new List and adds the chargingZone
        /// </summary>
        /// <param name="chargingZone">ChargingZone that is to be added</param>
        /// <returns>the added ChargingZone</returns>
        public ChargingZone Create(ChargingZone chargingZone)
        {
            if (_cache.TryGetValue("CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {
                createdChargingZones.Add(chargingZone);
                return chargingZone;
            }
            else
            {
                createdChargingZones = new List<ChargingZone> { chargingZone };
                _cache.Set("CreateChargingZone", createdChargingZones);
                return chargingZone;
            }
        }

        /// <summary>
        /// Delets the ChargingZone with specified Id
        /// </summary>
        /// <param name="_Id">ChargingZone Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdChargingZones.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of ChargingZones in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of ChargingZones</returns>
        public List<ChargingZone> GetAll()
        {
            if (_cache.TryGetValue("CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {
                return createdChargingZones;
            }
            else
            {
                createdChargingZones = new List<ChargingZone>();
                _cache.Set("CreateChargingZone", createdChargingZones);
                return createdChargingZones;
            }
        }

        /// <summary>
        /// Finds a ChargingZone with specified ID and returns it
        /// </summary>
        /// <param name="_Id">ChargingZone Id</param>
        /// <returns>ChargingZone with specified Id on success and null on failure</returns>
        public ChargingZone GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateChargingZone", out List<ChargingZone> createdChargingZones))
            {

                return createdChargingZones.Find(x => x.Chargingzone_Id == _Id);
            }
            else
            {
                return null;
            }
        }

    }
}
