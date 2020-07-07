using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface VehicleDao
    {
        // All usable Methods of VehicleDao
        // Implemented in VehicleDaoImpl
        Vehicle GetById(int id);
        List<Vehicle> GetAll();
        int Create(string model_name, double capacity, List<ConnectorType> connector_types);
        Vehicle Create(Vehicle vehicle);
        bool Delete(int id);
    }
}
