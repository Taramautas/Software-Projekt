using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface ChargingColumnTypeDao
    {
        // All usable Methods of ChargingColumnDao
        // Implemented in ChargingColumnTypeDaoImpl
        ChargingColumnType GetById(int Id);
        List<ChargingColumnType> GetAll();
        public int Create(int _max_concurrent_charging, string _manufacturer_name, List<ConnectorType> _connectors);
        bool Delete(int Id);


    }
}