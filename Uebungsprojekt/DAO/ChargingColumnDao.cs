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
        public int Create(ChargingColumnType _charging_column_type_id, Boolean _Busy, Boolean _Emergency_reserve, ChargingZone _charging_zone,List<Tuple<DateTime,DateTime>> _list, int DaoId);
        bool Delete(int Id, int DaoId);
    }
}
