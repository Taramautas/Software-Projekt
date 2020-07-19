using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface SimulationResultDao
    {
        // All usable Methods of SimulationResultDao
        // Implemented in SimulationResultDaoImpl
        SimulationResult GetById(int Id);
        List<SimulationResult> GetAll();
        int Create(SimulationConfig _config, SimulationInfrastructure _infrastructure, List<Dictionary<string, double>> _total_workload, List<int> _num_generated_bookings, List<int> _num_unsatisfiable_bookings, bool _done);
        bool Delete(int Id);
    }
}
