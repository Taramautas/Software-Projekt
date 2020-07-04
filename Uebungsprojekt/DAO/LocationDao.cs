using Uebungsprojekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uebungsprojekt.DAO
{
    public interface LocationDao
    {
        // All usable Methods of LocationDao
        // Implemented in LocationDaoImpl
        Location GetById(int Id);
        List<Location> GetAll();
        Location Create(Location location);
        bool Delete(int Id);
    }
}
