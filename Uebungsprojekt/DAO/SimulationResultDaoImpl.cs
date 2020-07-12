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
        /// Creates and adds a SimulationResult with new Id to the SimulationResultlist if there is one, else it creates a new List and adds the SimulationResult
        /// </summary>
        /// <returns>the id of the added SimulationResult</returns>
        public int Create(SimulationConfig _config, SimulationInfrastructure _infrastructure, List<double> _total_workload, List<int> _num_generated_bookings, List<int> _num_unsatisfiable_bookings, bool _done, List<Tuple<Booking, Booking>> _unsatisfiable_bookings_with_suggestion)
        {
            if (_cache.TryGetValue("CreateSimulationResultIds", out int ids))
            {
                ++ids;
                _cache.Set("CreateSimulationResultIds", ids);
                _cache.TryGetValue("CreateSimulationResult", out List<SimulationResult> createdSimulationResults);
                SimulationResult newSimulationResult = new SimulationResult()
                {
                    id = ids,
                    config = _config,
                    infrastructure = _infrastructure,
                    total_workload = _total_workload,
                    num_generated_bookings = _num_generated_bookings,
                    num_unsatisfiable_bookings = _num_unsatisfiable_bookings,
                    done = _done,
                    unsatisfiable_bookings_with_suggestion = _unsatisfiable_bookings_with_suggestion,
                };
                createdSimulationResults.Add(newSimulationResult);
                return ids;
            }

            else
            {
                List<SimulationResult> createdSimulationResults = new List<SimulationResult>();
                ids = 0;
                SimulationResult newSimulationResult = new SimulationResult()
                {
                    id = ++ids,
                    config = _config,
                    infrastructure = _infrastructure,
                    total_workload = _total_workload,
                    num_generated_bookings = _num_generated_bookings,
                    num_unsatisfiable_bookings = _num_unsatisfiable_bookings,
                    done = _done,
                    unsatisfiable_bookings_with_suggestion = _unsatisfiable_bookings_with_suggestion,
                };
                createdSimulationResults.Add(newSimulationResult);
                _cache.Set("CreateSimulationResult", createdSimulationResults);
                _cache.Set("CreateSimulationResultIds", ids);
                return ids;
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
