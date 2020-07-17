using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt
{
    public class HelpFunctions
    {
        
       
        /// <summary>
        /// Helpfunktion for searching after CharginColumns in a specific Location
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool FindLocationColumn(ChargingColumn chargingcolumn, Location location)
        {
            if(chargingcolumn.charging_zone.location.id == location.id)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Helpfunktion for searching after unaccapted Bookings
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public static bool FindUnacceptedBookings(Booking booking)
        {

            if (booking.accepted == false)
            {
                return true;
            }
            return false;
        }

        public static bool FindAcceptedBookings(Booking booking)
        {

            if (booking.accepted == true)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helfunktion to proof the connector type is equal to the booking connector type
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ConnectorCompare(ChargingColumn cc, Booking b)
        {
            foreach(Tuple<ConnectorType, int> tuple in cc.charging_column_type_id.connectors)
            {
                foreach(ConnectorType ctb in b.vehicle.connector_types)
                {
                    if(tuple.Item1 == ctb)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>> setList(List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>> _list, ChargingColumnType ct)
        {
            if(_list == null)
            {
                var listnew = new List<Tuple<List<Tuple<DateTime, DateTime>>, ConnectorType>>();
                foreach (Tuple<ConnectorType, int> tuple in ct.connectors)
                {
                    listnew.Add(Tuple.Create(new List<Tuple<DateTime, DateTime>>(), tuple.Item1));
                }
                return listnew;  
            }
            else
            {
                return _list;
            }
        }
    }

   

   

}

