using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class SimulationResultDaoImpl : SimulationResultDao
    {
        private IMemoryCache _cache;

        public SimulationResultDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a simulationResult to the SimulationResultlist if there is one, else it creates a new List and adds the simulationResult
        /// </summary>
        /// <param name="simulationResult">SimulationResult that is to be added</param>
        /// <returns>the added SimulationResult</returns>
        public SimulationResult Create(SimulationResult simulationResult)
        {
            if (_cache.TryGetValue("CreateSimulationResult", out List<SimulationResult> createdSimulationResults))
            {
                createdSimulationResults.Add(simulationResult);
                return simulationResult;
            }
            else
            {
                createdSimulationResults = new List<SimulationResult> { simulationResult };
                _cache.Set("CreateSimulationResult", createdSimulationResults);
                return simulationResult;
            }
        }

        /// <summary>
        /// Delets the SimulationResult with specified Id
        /// </summary>
        /// <param name="_Id">SimulationResult Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateSimulationResult", out List<SimulationResult> createdSimulationResults))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdSimulationResults.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of SimulationResults in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of SimulationResults</returns>
        public List<SimulationResult> GetAll()
        {
            if (_cache.TryGetValue("CreateSimulationResult", out List<SimulationResult> createdSimulationResults))
            {
                return createdSimulationResults;
            }
            else
            {
                createdSimulationResults = new List<SimulationResult>();
                _cache.Set("CreateSimulationResult", createdSimulationResults);
                return createdSimulationResults;
            }
        }

        /// <summary>
        /// Finds a SimulationResult with specified ID and returns it
        /// </summary>
        /// <param name="_Id">SimulationResult Id</param>
        /// <returns>SimulationResult with specified Id on success and null on failure</returns>
        public SimulationResult GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateSimulationResult", out List<SimulationResult> createdSimulationResults))
            {

                return createdSimulationResults.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}
