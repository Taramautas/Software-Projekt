using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class ChargingColumnDaoImpl : ChargingColumnDao
    {
        private IMemoryCache _cache;

        public ChargingColumnDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a chargingColumn to the ChargingColumnlist if there is one, else it creates a new List and adds the chargingColumn
        /// </summary>
        /// <param name="ChargingColumn">ChargingColumn that is to be added</param>
        /// <returns>the added ChargingColumn</returns>
        public ChargingColumn Create(ChargingColumn chargingColumn)
        {
            if (_cache.TryGetValue("CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {
                createdChargingColumns.Add(chargingColumn);
                return chargingColumn;
            }
            else
            {
                createdChargingColumns = new List<ChargingColumn> { chargingColumn };
                _cache.Set("CreateChargingColumn", createdChargingColumns);
                return chargingColumn;
            }
        }


        /// <summary>
        /// Delets the ChargingColumn with specified Id
        /// </summary>
        /// <param name="_Id">ChargingColumn Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdChargingColumns.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of ChargingColumns in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of ChargingColumns</returns>
        public List<ChargingColumn> GetAll()
        {
            if (_cache.TryGetValue("CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
            {
                return createdChargingColumns;
            }
            else
            {
                createdChargingColumns = new List<ChargingColumn>();
                _cache.Set("CreateChargingColumn", createdChargingColumns);
                return createdChargingColumns;
            }
        }

        /// <summary>
        /// Finds a ChargingColumn with specified ID and returns it
        /// </summary>
        /// <param name="_Id">ChargingColumn Id</param>
        /// <returns>ChargingColumn with specified Id on success and null on failure</returns>
        public ChargingColumn GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateChargingColumn", out List<ChargingColumn> createdChargingColumns))
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
