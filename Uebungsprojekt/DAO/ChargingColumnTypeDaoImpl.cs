using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class ChargingColumnTypeDaoImpl : ChargingColumnTypeDao
    {
        private IMemoryCache _cache;

        public ChargingColumnTypeDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Creates and adds a User with new Id to the Userlist if there is one, else it creates a new List and adds the User
        /// </summary>
        /// <returns>the id of the added User</returns>
        public int Create(string _model_name, string _manufacturer_name, int _max_concurrent_charging, List<Tuple<ConnectorType, int>> _connectors, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumnTypeIds", out int ids))
            {
                ++ids;
                _cache.Set(DaoId + "CreateChargingColumnTypeIds", ids);
                _cache.TryGetValue(DaoId + "CreateChargingColumnType", out List<ChargingColumnType> created_charging_column_types);
                ChargingColumnType new_charging_column_type = new ChargingColumnType
                {
                    id = ids,
                    model_name = _model_name,
                    manufacturer_name = _manufacturer_name,
                    max_parallel_charging = _max_concurrent_charging,
                    connectors = _connectors,
                };
                created_charging_column_types.Add(new_charging_column_type);
                return ids;
            }

            else
            {
                List<ChargingColumnType> created_charging_column_types = new List<ChargingColumnType>();
                ids = 0;
                ChargingColumnType new_charging_column_type = new ChargingColumnType
                {
                    id = ids,
                    model_name = _model_name,
                    max_parallel_charging = _max_concurrent_charging,
                    manufacturer_name = _manufacturer_name,
                    connectors = _connectors,
                };
                created_charging_column_types.Add(new_charging_column_type);
                _cache.Set(DaoId + "CreateChargingColumnType", created_charging_column_types);
                _cache.Set(DaoId + "CreateChargingColumnTypeIds", ids);
                return ids;
            }
        }

        
        /// <summary>
        /// Delets the User with specified Id
        /// </summary>
        /// <param name="_Id">User Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumnType", out List<ChargingColumnType> created_charging_column_types))
            {
                if (GetById(_Id, DaoId) == null)
                {
                    return false;
                }
                created_charging_column_types.Remove(GetById(_Id, DaoId));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Users in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of Users</returns>
        public List<ChargingColumnType> GetAll(int DaoID)
        {
            if (_cache.TryGetValue(DaoID + "CreateChargingColumnType", out List<ChargingColumnType> created_charging_column_types))
            {
                return created_charging_column_types;
            }
            else
            {
                created_charging_column_types = new List<ChargingColumnType>();
                _cache.Set(DaoID + "CreateChargingColumnType", created_charging_column_types);
                return created_charging_column_types;
            }
        }

        /// <summary>
        /// Finds a User with specified ID and returns it
        /// </summary>
        /// <param name="_Id">User Id</param>
        /// <returns>User with specified Id on success and null on failure</returns>
        public ChargingColumnType GetById(int _Id, int DaoId)
        {
            if (_cache.TryGetValue(DaoId + "CreateChargingColumnType", out List<ChargingColumnType> createdUsers))
            {

                return createdUsers.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
