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
            double d = (60 * ((((double)booking.vehicle.capacity / 100) * (double)booking.target_state_of_charge - ((double)booking.vehicle.capacity / 100) * (double)booking.start_state_of_charge) / (double)chargingcolumn.max_concurrent_charging));
            TimeSpan time = new TimeSpan(0, (int)d,0);
            if (time < (bookingTime))
            {
                return time;
            }
            return bookingTime;
        }

    }       
}
