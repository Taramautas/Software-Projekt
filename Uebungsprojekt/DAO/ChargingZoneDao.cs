using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface ChargingZoneDao
    {
        // All usable Methods of ChargingZoneDao
        // Implemented in ChargingZoneDaoImpl
        ChargingZone GetById(int Id, int DaoId);
        List<ChargingZone> GetAll(int DaoId);
        public int Create(int _Overall_performance, int DaoId);
        bool Delete(int Id, int DaoId);
    }
}
