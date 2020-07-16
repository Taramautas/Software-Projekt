using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using static Uebungsprojekt.Models.Booking;
using static Uebungsprojekt.Models.ChargingColumn;

namespace Uebungsprojekt.DAO
{
    public class ChargingColumnDaoImpl : ChargingColumnDao
    {
        private IMemoryCache _cache;
        private static int DaoId = 0;

        public ChargingColumnDaoImpl(IMemoryCache memoryCache)
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
        /// Creates and adds a chargingColumn to the ChargingColumnlist if there is one, else it creates a new List and adds the chargingColumn
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the id of the added ChargingColumn</returns>
        public int Create(ChargingColumnType _charging_column_type_id, ChargingZone _charging_zone, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumnIds", out int ids))
            {
                ++ids;
                _cache.Set(DaoId + "CreateChargingColumnIds", ids);
                _cache.TryGetValue(DaoId + "CreateChargingColumn", out List<ChargingColumn> createdChargingColumns);
                ChargingColumn newChargingColumn = new ChargingColumn
                {
                    id = ids,
                    charging_column_type_id =_charging_column_type_id,
                    busy = false,
                    emergency_reserve = false,
                    charging_zone = _charging_zone,
                    list = new List<Tuple<DateTime, DateTime>>()
                };
                createdChargingColumns.Add(newChargingColumn);
                return ids;
            }

            else
            {
                List<ChargingColumn> createdChargingColumns = new List<ChargingColumn>();
                ids = 0;
                ChargingColumn newChargingColumn = new ChargingColumn
                {
                    id = ++ids,
                    charging_column_type_id =_charging_column_type_id,
                    emergency_reserve = false,
                    busy = false,
                    charging_zone = _charging_zone,
                    list = new List<Tuple<DateTime, DateTime>>()
                };
                createdChargingColumns.Add(newChargingColumn);
                _cache.Set(DaoId + "CreateChargingColumn", createdChargingColumns);
                _cache.Set(DaoId + "CreateChargingColumnIds", ids);
                return ids;
            }
        } 

        
        /// <summary>
        /// Delets the ChargingColumn with specified Id
        /// </summary>
        /// <param name="_Id">ChargingColumn Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {
                if (GetById(_Id, DaoId) == null)
                {
                    return false;
                }
                createdChargingColumns.Remove(GetById(_Id, DaoId));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of ChargingColumns in Cache with specified id if there is one, else it creates a new List and returns it
        /// </summary>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>List of ChargingColumns</returns>
        public List<ChargingColumn> GetAll(int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {
                return createdChargingColumns;
            }
            else
            {
                int ids = 0;
                createdChargingColumns = new List<ChargingColumn>();
                _cache.Set(DaoId + "CreateChargingColumn", createdChargingColumns);
                _cache.Set(DaoId + "CreateChargingColumnIds", ids);
                return createdChargingColumns;
            }
        }


        /// <summary>
        /// Finds a ChargingColumn with specified ID and returns it
        /// </summary>
        /// <param name="_Id">ChargingColumn Id</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>ChargingColumn with specified Id on success and null on failure</returns>
        public ChargingColumn GetById(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {

                return createdChargingColumns.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
