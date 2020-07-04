using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface SimulationInfrastructureDao
    {
        // All usable Methods of SimulationInfrastructureDao
        // Implemented in SimulationInfrastructureDaoImpl
        SimulationInfrastructure GetById(int Id);
        List<SimulationInfrastructure> GetAll();
        SimulationInfrastructure Create(SimulationInfrastructure simulationInfrastructure);
        bool Delete(int Id);
    }
}
