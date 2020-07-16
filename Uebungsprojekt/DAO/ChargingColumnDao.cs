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
        public int Create(ChargingColumnType _charging_column_type_id, ChargingZone _charging_zone, List<Tuple<List<Tuple<DateTime,DateTime>>, ConnectorType>> _list, int DaoId);
        bool Delete(int Id, int DaoId);
    }
}
