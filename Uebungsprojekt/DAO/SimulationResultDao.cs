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
        SimulationResult Create(SimulationResult simulationResult);
        bool Delete(int Id);
    }
}
