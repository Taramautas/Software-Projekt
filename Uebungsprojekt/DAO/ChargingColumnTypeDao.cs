using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface ChargingColumnTypeDao
    {
        // All usable Methods of ChargingColumnDao
        // Implemented in ChargingColumnTypeDaoImpl
        ChargingColumnType GetById(int Id, int DaoId);
        List<ChargingColumnType> GetAll(int DaoId);
        public int Create(string _model_name, string _manufacturer_name, int _max_concurrent_charging, List<Tuple<ConnectorType, int>> _connectors, int DaoId);
        bool Delete(int Id, int DaoId);


    }
}