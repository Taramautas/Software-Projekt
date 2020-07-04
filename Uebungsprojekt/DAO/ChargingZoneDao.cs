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
        ChargingZone GetById(int Id);
        List<ChargingZone> GetAll();
        ChargingZone Create(ChargingZone chargingZone);
        bool Delete(int Id);
    }
}
