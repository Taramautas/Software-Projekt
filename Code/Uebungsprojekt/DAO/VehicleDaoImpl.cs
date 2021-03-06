using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class VehicleDaoImpl : VehicleDao
    {
        private IMemoryCache _cache;

        public VehicleDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Creates and adds a Vehicle with new Id to the Vehiclelist if there is one, else it creates a new List and adds the Vehicle
        /// </summary>
        /// <returns>the id of the added Vehicle</returns>
        public int Create(string _model_name, int _capacity, List<ConnectorType> _connector_types, User _user)
        {
            if (_cache.TryGetValue("CreateVehicleIds", out int ids))
            {
                ++ids;
                _cache.Set("CreateVehicleIds", ids);
                _cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles);
                Vehicle newVehicle = new Vehicle
                {
                    id = ids,
                    model_name = _model_name,
                    capacity = _capacity,
                    connector_types = _connector_types,
                    user = _user
                };
                createdVehicles.Add(newVehicle);
                return ids;
            }

            else
            {
                List<Vehicle> createdVehicles = new List<Vehicle>();
                ids = 0;
                Vehicle newVehicle = new Vehicle
                {
                    id = ++ids,
                    model_name = _model_name,
                    capacity = _capacity,
                    connector_types = _connector_types,
                    user = _user
                };
                createdVehicles.Add(newVehicle);
                _cache.Set("CreateVehicle", createdVehicles);
                _cache.Set("CreateVehicleIds", ids);
                return ids;
            }
        }

        
        /// <summary>
        /// Delets the Vehicle with specified Id
        /// </summary>
        /// <param name="_Id">Vehicle Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdVehicles.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Vehicles in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of Vehicles</returns>
        public List<Vehicle> GetAll()
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {
                return createdVehicles;
            }
            else
            {
                createdVehicles = new List<Vehicle>();
                _cache.Set("CreateVehicle", createdVehicles);
                return createdVehicles;
            }
        }

        /// <summary>
        /// Finds a Vehicle with specified ID and returns it
        /// </summary>
        /// <param name="_Id">Vehicle Id</param>
        /// <returns>Vehicle with specified Id on success and null on failure</returns>
        public Vehicle GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {

                return createdVehicles.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds all Vehicles from specified user and returns them
        /// </summary>
        /// <param name="user_id">the id of the user whose bookings we are looking for</param>
        /// <returns>List of Vehicles with specified parameters on success and null on failure</returns>
        public List<Vehicle> GetVehiclesByUserId(int user_id)
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {
                List<Vehicle> vehicles_by_user = new List<Vehicle>();
                foreach (Vehicle v in createdVehicles)
                {
                    if (v.user != null)
                    {
                        if ((v.user.id == user_id))
                        {
                            vehicles_by_user.Add(v);
                        }
                    }
                }
                return vehicles_by_user;
            }
            else
            {
                return null;
            }
        }
    }
}
