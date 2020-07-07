using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class SimulationInfrastructureDaoImpl : SimulationInfrastructureDao
    {
        private IMemoryCache _cache;

        public SimulationInfrastructureDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Creates and adds a SimulationInfrastructure with new Id to the SimulationInfrastructurelist if there is one, else it creates a new List and adds the SimulationInfrastructure
        /// </summary>
        /// <returns>the id of the added SimulationInfrastructure</returns>
        public int Create(LocationDaoImpl _location_dao, ChargingZoneDaoImpl _charging_zone_dao, ChargingColumnDaoImpl _charging_column_dao, BookingDaoImpl _booking_dao)
        {
            if (_cache.TryGetValue("CreateSimulationInfrastructureIds", out int ids))
            {
                ++ids;
                _cache.Set("CreateSimulationInfrastructureIds", ids);
                _cache.TryGetValue("CreateSimulationInfrastructure", out List<SimulationInfrastructure> createdSimulationInfrastructures);
                SimulationInfrastructure newSimulationInfrastructure = new SimulationInfrastructure
                {
                    id = ids,
                    location_dao = _location_dao,
                    charging_zone_dao = _charging_zone_dao,
                    charging_column_dao = _charging_column_dao,
                    booking_dao = _booking_dao,
                };
                createdSimulationInfrastructures.Add(newSimulationInfrastructure);
                return ids;
            }

            else
            {
                List<SimulationInfrastructure> createdSimulationInfrastructures = new List<SimulationInfrastructure>();
                ids = 0;
                SimulationInfrastructure newSimulationInfrastructure = new SimulationInfrastructure
                {
                    id = ++ids,
                    location_dao = _location_dao,
                    charging_zone_dao = _charging_zone_dao,
                    charging_column_dao = _charging_column_dao,
                    booking_dao = _booking_dao,
                };
                createdSimulationInfrastructures.Add(newSimulationInfrastructure);
                _cache.Set("CreateSimulationInfrastructure", createdSimulationInfrastructures);
                _cache.Set("CreateSimulationInfrastructureIds", ids);
                return ids;
            }
        }

        /// <summary>
        /// Adds a simulationInfrastructure to the SimulationInfrastructurelist if there is one, else it creates a new List and adds the simulationInfrastructure
        /// </summary>
        /// <param name="simulationInfrastructure">SimulationInfrastructure that is to be added</param>
        /// <returns>the added SimulationInfrastructure</returns>
        public SimulationInfrastructure Create(SimulationInfrastructure simulationInfrastructure)
        {
            if (_cache.TryGetValue("CreateSimulationInfrastructure", out List<SimulationInfrastructure> createdSimulationInfrastructures))
            {
                createdSimulationInfrastructures.Add(simulationInfrastructure);
                return simulationInfrastructure;
            }
            else
            {
                createdSimulationInfrastructures = new List<SimulationInfrastructure> { simulationInfrastructure };
                _cache.Set("CreateSimulationInfrastructure", createdSimulationInfrastructures);
                return simulationInfrastructure;
            }
        }

        /// <summary>
        /// Delets the SimulationInfrastructure with specified Id
        /// </summary>
        /// <param name="_Id">SimulationInfrastructure Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateSimulationInfrastructure", out List<SimulationInfrastructure> createdSimulationInfrastructures))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdSimulationInfrastructures.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of SimulationInfrastructures in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of SimulationInfrastructures</returns>
        public List<SimulationInfrastructure> GetAll()
        {
            if (_cache.TryGetValue("CreateSimulationInfrastructure", out List<SimulationInfrastructure> createdSimulationInfrastructures))
            {
                return createdSimulationInfrastructures;
            }
            else
            {
                createdSimulationInfrastructures = new List<SimulationInfrastructure>();
                _cache.Set("CreateSimulationInfrastructure", createdSimulationInfrastructures);
                return createdSimulationInfrastructures;
            }
        }

        /// <summary>
        /// Finds a SimulationInfrastructure with specified ID and returns it
        /// </summary>
        /// <param name="_Id">SimulationInfrastructure Id</param>
        /// <returns>SimulationInfrastructure with specified Id on success and null on failure</returns>
        public SimulationInfrastructure GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateSimulationInfrastructure", out List<SimulationInfrastructure> createdSimulationInfrastructures))
            {

                return createdSimulationInfrastructures.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
