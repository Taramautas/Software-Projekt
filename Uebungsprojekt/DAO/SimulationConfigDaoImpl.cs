using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class SimulationConfigDaoImpl : SimulationConfigDao
    {
        private IMemoryCache _cache;

        public SimulationConfigDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Creates and adds a SimulationConfig with new Id to the SimulationConfiglist if there is one, else it creates a new List and adds the SimulationConfig
        /// </summary>
        /// <returns>the id of the added SimulationConfig</returns>
        public int Create(int _tick_minutes, List<Tuple<DayOfWeek, TimeSpan>> _rush_hours, int _min, int _max, double _spread, int _weeks, List<Vehicle> _vehicles)
        {
            if (_cache.TryGetValue("CreateSimulationConfigIds", out int ids))
            {
                ++ids;
                _cache.Set("CreateSimulationConfigIds", ids);
                _cache.TryGetValue("CreateSimulationConfig", out List<SimulationConfig> createdSimulationConfigs);
                SimulationConfig newSimulationConfig = new SimulationConfig
                {
                    id = ids,
                    tick_minutes = _tick_minutes,
                    rush_hours = _rush_hours,
                    min = _min,
                    max = _max,
                    spread = _spread,
                    weeks = _weeks,
                    vehicles = _vehicles,
                };
                createdSimulationConfigs.Add(newSimulationConfig);
                return ids;
            }

            else
            {
                List<SimulationConfig> createdSimulationConfigs = new List<SimulationConfig>();
                ids = 0;
                SimulationConfig newSimulationConfig = new SimulationConfig
                {
                    id = ++ids,
                    tick_minutes = _tick_minutes,
                    rush_hours = _rush_hours,
                    min = _min,
                    max = _max,
                    spread = _spread,
                    weeks = _weeks,
                    vehicles = _vehicles,
                };
                createdSimulationConfigs.Add(newSimulationConfig);
                _cache.Set("CreateSimulationConfig", createdSimulationConfigs);
                _cache.Set("CreateSimulationConfigIds", ids);
                return ids;
            }
        }

        /// <summary>
        /// Adds a simulationConfig to the SimulationConfiglist if there is one, else it creates a new List and adds the simulationConfig
        /// </summary>
        /// <param name="simulationConfig">SimulationConfig that is to be added</param>
        /// <returns>the added SimulationConfig</returns>
        public SimulationConfig Create(SimulationConfig simulationConfig)
        {
            if (_cache.TryGetValue("CreateSimulationConfig", out List<SimulationConfig> createdSimulationConfigs))
            {
                createdSimulationConfigs.Add(simulationConfig);
                return simulationConfig;
            }
            else
            {
                createdSimulationConfigs = new List<SimulationConfig> { simulationConfig };
                _cache.Set("CreateSimulationConfig", createdSimulationConfigs);
                return simulationConfig;
            }
        }

        /// <summary>
        /// Delets the SimulationConfig with specified Id
        /// </summary>
        /// <param name="_Id">SimulationConfig Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateSimulationConfig", out List<SimulationConfig> createdSimulationConfigs))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdSimulationConfigs.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of SimulationConfigs in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of SimulationConfigs</returns>
        public List<SimulationConfig> GetAll()
        {
            if (_cache.TryGetValue("CreateSimulationConfig", out List<SimulationConfig> createdSimulationConfigs))
            {
                return createdSimulationConfigs;
            }
            else
            {
                createdSimulationConfigs = new List<SimulationConfig>();
                _cache.Set("CreateSimulationConfig", createdSimulationConfigs);
                return createdSimulationConfigs;
            }
        }

        /// <summary>
        /// Finds a SimulationConfig with specified ID and returns it
        /// </summary>
        /// <param name="_Id">SimulationConfig Id</param>
        /// <returns>SimulationConfig with specified Id on success and null on failure</returns>
        public SimulationConfig GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateSimulationConfig", out List<SimulationConfig> createdSimulationConfigs))
            {

                return createdSimulationConfigs.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
