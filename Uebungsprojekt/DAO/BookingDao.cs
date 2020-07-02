using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface BookingDao
    {
        // All usable Methods of BookingDao
        // Implemented in BookingDaoImpl
        Booking GetById(int Id, int DaoId);
        List<Booking> GetAll(int DaoId);
        Booking Create(Booking booking, int DaoId);
        bool Delete(int Id, int DaoId);

    }
}
