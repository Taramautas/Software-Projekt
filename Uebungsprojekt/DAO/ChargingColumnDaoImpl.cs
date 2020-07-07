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
        public int Create(Boolean _Busy, string _Manufacturer_name, Dictionary<ConnectorType, ConnectorType> _Connectors, Boolean _Emergency_reserve, int _Max_concurrent_charging, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumnIds", out int ids))
            {
                ++ids;
                _cache.Set(DaoId + "CreateChargingColumnIds", ids);
                _cache.TryGetValue(DaoId + "CreateChargingColumn", out List<ChargingColumn> createdChargingColumns);
                ChargingColumn newChargingColumn = new ChargingColumn
                {
                    Id = ids,
                    Busy = _Busy,
                    Manufacturer_name = _Manufacturer_name,
                    Connectors = _Connectors,
                    Emergency_reserve = _Emergency_reserve,
                    Max_concurrent_charging = _Max_concurrent_charging,
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
                    Id = ++ids,
                    Busy = _Busy,
                    Manufacturer_name = _Manufacturer_name,
                    Connectors = _Connectors,
                    Emergency_reserve = _Emergency_reserve,
                    Max_concurrent_charging = _Max_concurrent_charging,
                };
                createdChargingColumns.Add(newChargingColumn);
                _cache.Set(DaoId + "CreateChargingColumn", createdChargingColumns);
                _cache.Set(DaoId + "CreateChargingColumnIds", ids);
                return ids;
            }
        } // Frage: Soll Create überhaupt in der Lage sein eine neue DaoListe anzulegen falls die id nicht vorhanden ist?

        /// <summary>
        /// Adds a chargingColumn to the ChargingColumnlist if there is one, else it creates a new List and adds the chargingColumn
        /// </summary>
        /// <param name="chargingColumn">ChargingColumn that is to be added</param>
        /// <param name="DaoId">Id of List that's to be used.</param>
        /// <returns>the added ChargingColumn</returns>
        public ChargingColumn Create(ChargingColumn chargingColumn, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {
                createdChargingColumns.Add(chargingColumn);
                return chargingColumn;
            }
            else
            {
                createdChargingColumns = new List<ChargingColumn> { chargingColumn };
                _cache.Set(DaoId + "CreateChargingColumn", createdChargingColumns);
                return chargingColumn;
            }
        } // Frage: Soll das so überhaupt bestehen bleiben?

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

                return createdChargingColumns.Find(x => x.Id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
