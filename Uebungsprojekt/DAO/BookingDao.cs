using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using static Uebungsprojekt.Models.Booking;

namespace Uebungsprojekt.DAO
{
    public interface BookingDao
    {
        // All usable Methods of BookingDao
        // Implemented in BookingDaoImpl
        Booking GetById(int Id, int DaoId);
        List<Booking> GetAll(int DaoId);
        public int Create(int _start_state_of_charge, int _target_state_of_charge, DateTime _start_time, DateTime _end_time, Vehicle _vehicle, User _user, Location _location, ChargingColumn chargingColumn, int DaoId);
        bool Delete(int Id, int DaoId);
        public List<Booking> GetOpenBookingsByUserId(int user_id);
        public List<Booking> GetAcceptedBookingsByUserId(int user_id);
    }
}
