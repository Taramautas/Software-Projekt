using System;
using System.Collections.Generic;
using System.Diagnostics;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.OccupancyPlans
{
    public class OccupancyPlan
    {

        public OccupancyPlan(ChargingColumnDaoImpl chargingColumnDaoImpl)
        {



            List<Tuple<List<Tuple<DateTime,DateTime>>,ChargingColumn>> overallOccupancyPlan = new List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>>();

            foreach(ChargingColumn cc in chargingColumnDaoImpl.GetAll(0))
            {
                overallOccupancyPlan.Add(new Tuple<List<Tuple<DateTime,DateTime>>,ChargingColumn>(cc.list, cc));
            }

           
            
        }

        public static List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> LocationFilter(List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> list, Location location)
        {
            List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> locationOccupancyPlan = list.FindAll(delegate (Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn> tuple)
            {
                 return tuple.Item2.charging_zone.location.id == location.id;
            });

            return locationOccupancyPlan;
        }
            
        



    }
}
