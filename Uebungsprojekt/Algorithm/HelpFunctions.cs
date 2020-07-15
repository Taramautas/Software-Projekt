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
        /// Helpfunktion for searching after Schuko Socket
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <returns></returns>
        public static bool FindSchuko_Socket(ChargingColumn chargingcolumn)
        {
            foreach(ConnectorType ct in chargingcolumn.charging_column_type_id.connectors)
            {
                if(ct == ConnectorType.Schuko_Socket)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Helfunktion for searching after Type 1 Plug
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <returns></returns>
        public static bool FindType_1_Plug(ChargingColumn chargingcolumn)
        {
            foreach (ConnectorType ct in chargingcolumn.charging_column_type_id.connectors)
            {
                if (ct == ConnectorType.Type_1_Plug)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Helpfunktion for searching after Type 2 Plug
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <returns></returns>
        public static bool FindType_2_Plug(ChargingColumn chargingcolumn)
        {
            foreach (ConnectorType ct in chargingcolumn.charging_column_type_id.connectors)
            {
                if (ct == ConnectorType.Type_2_Plug)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Helpfunktion for searching after ChAdeMO Plug
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <returns></returns>
        public static bool FindCHAdeMO_Plug(ChargingColumn chargingcolumn)
        {
            foreach (ConnectorType ct in chargingcolumn.charging_column_type_id.connectors)
            {
                if (ct == ConnectorType.CHAdeMO_Plug)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Helpfunktion for searching after Tesla Supercharger
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <returns></returns>
        public static bool FindTesla_Supercharger(ChargingColumn chargingcolumn)
        {
            foreach (ConnectorType ct in chargingcolumn.charging_column_type_id.connectors)
            {
                if (ct == ConnectorType.Tesla_Supercharger)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Helpfunktion for searching after Combo 2 Plug
        /// </summary>
        /// <param name="chargingcolumn"></param>
        /// <returns></returns>
        public static bool FindCCS_Combo_2_Plug(ChargingColumn chargingcolumn)
        {
            foreach (ConnectorType ct in chargingcolumn.charging_column_type_id.connectors)
            {
                if (ct == ConnectorType.CCS_Combo_2_Plug)
                {
                    return true;
                }
            }
            return false;
        }
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

        /// <summary>
        /// Helfunktion to proof the connector type is equal to the booking connector type
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ConnectorCompare(ChargingColumn cc, Booking b)
        {
            foreach(ConnectorType ct in cc.charging_column_type_id.connectors)
            {
                foreach(ConnectorType ctb in b.vehicle.connector_types)
                {
                    if(ct == ctb)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

   

   

}

