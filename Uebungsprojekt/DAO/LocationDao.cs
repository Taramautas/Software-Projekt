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
        Location GetById(int Id, int DaoId);
        List<Location> GetAll(int DaoId);
        public int Create(string _City, string _Post_code, string _Adress, int DaoId);
        Location Create(Location location, int DaoId);
        bool Delete(int Id, int DaoId);
    }
}
