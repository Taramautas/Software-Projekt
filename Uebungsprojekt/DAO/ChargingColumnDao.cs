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
        ChargingColumn GetById(int Id);
        List<ChargingColumn> GetAll();
        ChargingColumn Create(ChargingColumn chargingColumn);
        bool Delete(int Id);
    }
}
