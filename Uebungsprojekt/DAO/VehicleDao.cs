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
        int Create(string _model_name, double _capacity, List<ConnectorType> _connector_types);
        bool Delete(int id);
    }
}
