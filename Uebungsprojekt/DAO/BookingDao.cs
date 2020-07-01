using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public interface BookingDao
    {
        // All usable Methods of BookingDao
        // Implemented in BookingDaoImpl
        Booking GetById(int Id);
        List<Booking> GetAll();
        Booking Create(Booking booking);
        bool Delete(int Id);

    }
}
