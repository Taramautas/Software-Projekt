using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface SimulationConfigDao
    {
        // All usable Methods of SimulationConfigDao
        // Implemented in SimulationConfigDaoImpl
        SimulationConfig GetById(int Id);
        List<SimulationConfig> GetAll();
        SimulationConfig Create(SimulationConfig simulationConfig);
        bool Delete(int Id);
    }
}
