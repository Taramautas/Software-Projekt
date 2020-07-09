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
        int Create(int _location_dao_id, int _charging_zone_dao_id, int _charging_column_dao_id, int _booking_dao_id);
        bool Delete(int Id);
    }
}
