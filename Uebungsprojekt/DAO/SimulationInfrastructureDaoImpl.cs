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
