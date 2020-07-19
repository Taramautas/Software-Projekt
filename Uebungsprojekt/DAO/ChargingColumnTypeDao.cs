using System;
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
        public int Create(string _model_name, string _manufacturer_name, int _max_parallel_charging, List<Tuple<ConnectorType, int>> _connectors);
        bool Delete(int Id);


    }
}