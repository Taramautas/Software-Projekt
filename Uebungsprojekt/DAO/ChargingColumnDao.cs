using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface ChargingColumnDao
    {
        // All usable Methods of ChargingColumnDao
        // Implemented in ChargingColumnDaoImpl
        ChargingColumn GetById(int Id, int DaoId);
        List<ChargingColumn> GetAll(int DaoId);
        public int Create(Boolean _Busy, string _Manufacturer_name, Dictionary<ConnectorType, ConnectorType> _Connectors, Boolean _Emergency_reserve, int _Max_concurrent_charging, int DaoId);
        ChargingColumn Create(ChargingColumn chargingColumn, int DaoId);
        bool Delete(int Id, int DaoId);
    }
}
