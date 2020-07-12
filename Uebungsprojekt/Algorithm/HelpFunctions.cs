using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Algorithm
{
    public class HelpFunctions
    {
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

        public static bool FindLocationColumn(ChargingColumn chargingcolumn, Location location)
        {
            if(chargingcolumn.charging_zone.location.id == location.id)
            {
                return true;
            }
            return false;
        }

            public static bool FindUnacceptetBookings(Booking booking)
        {

            if (booking.accepted == true)
            {
                return true;
            }
            return false;
        }


        public static bool ConnectorCompare(ChargingColumn cc, Booking b)
        {
            foreach(ConnectorType ct in cc.charging_column_type_id.connectors)
            {
                foreach(ConnectorType ctb in b.connectorTypes)
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

