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
        int Create(int _tick_minutes, List<Tuple<DayOfWeek, TimeSpan>> _rush_hours, int _min, int _max, double _spread, int _weeks, List<Vehicle> _vehicles);
        SimulationConfig Create(SimulationConfig simulationConfig);
        bool Delete(int Id);
    }
}
