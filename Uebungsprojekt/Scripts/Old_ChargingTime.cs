using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.Algorithm;

namespace Uebungsprojekt.Algorithm
{
    public class ChargingTime
    {
        /// <summary>
        /// calculate the true charging time for a vehicle
        /// </summary>
        /// <param name="vehicle">extracted the maximum of the battery capacity</param>
        /// <param name="chargingcolumn">extracted the maximum charging power</param>
        /// <param name="booking"> extracted the timespan of booking and the start SoC</param>
        /// <returns></returns>
        public static TimeSpan RealChargingTime(ChargingColumnType chargingcolumn, Booking booking)
        {
            TimeSpan bookingTime = booking.end_time - booking.start_time;
            TimeSpan time = new TimeSpan((int)((booking.vehicle.capacity / chargingcolumn.max_concurrent_charging) * (double)((100 - booking.start_state_of_charge) / 100)));
            if (time < (bookingTime))
            {
                return time;
            }
            return bookingTime;
        }

    }       
}