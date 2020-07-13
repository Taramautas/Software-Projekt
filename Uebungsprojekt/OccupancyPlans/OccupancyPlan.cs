using System;
using System.Collections.Generic;
using System.Diagnostics;
using Uebungsprojekt.Algorithm;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.OccupancyPlans
{
    public class OccupancyPlan
    {


        private ChargingColumnDaoImpl chargingColumnDaoImpl;
        private List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> overallOccupancyPlan;
        /// <summary>
        /// Constructor with the occupancy plans for all charging columns
        /// </summary>
        /// <param name="chargingColumnDaoImpl"></param>
        public OccupancyPlan(ChargingColumnDaoImpl _chargingColumnDaoImpl)
        {
            chargingColumnDaoImpl = _chargingColumnDaoImpl;

            
            List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> overallOccupancyPlan = new List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>>();

            foreach (ChargingColumn cc in chargingColumnDaoImpl.GetAll(0))
            {
                overallOccupancyPlan.Add(new Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>(cc.list, cc));
            }



        }
        /// <summary>
        /// Filter for charging columns in a specific location
        /// </summary>
        /// <param name="list"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> LocationFilter(List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> list, Location location)
        {
            List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> locationOccupancyPlan = list.FindAll(delegate (Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn> tuple)
            {
                return tuple.Item2.charging_zone.location.id == location.id;
            });

            return locationOccupancyPlan;
        }
        /// <summary>
        /// funkion for accepting an adhoc booking 
        /// accept only if the booking request was made by the same day
        /// </summary>
        /// <param name="list"></param>
        /// <param name="b"></param>
        public static void AcceptAdHocBooking(List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> list, Booking b)
        {
            if (DateTime.Now.Year == b.start_time.Year && DateTime.Now.Month == b.start_time.Month && DateTime.Now.Day == b.start_time.Day)
            {
                foreach (Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn> tuple in list)
                {
                    if (tuple.Item2.id == b.charging_column.id)
                    {
                        int index = 0;
                        foreach (Tuple<DateTime, DateTime> tuple1 in tuple.Item1)
                        {
                            if (tuple1.Item1 > b.end_time && tuple.Item1[index - 1].Item1 < b.start_time)
                            {
                                tuple.Item2.list.Insert(index - 1, new Tuple<DateTime, DateTime>(b.start_time, b.end_time));
                                b.charging_column = tuple.Item2;
                                b.Accept();
                                return;
                            }
                        }

                    }
                }
            }
        }
        /// <summary>
        /// funkion for accepting an adhoc booking 
        /// accept only if the booking request was made by the same day
        /// </summary>
        /// <param name="list"></param>
        /// <param name="b"></param>
        public static void AcceptAdHocBooking(Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn> tuple, Booking b)
        {
            if (DateTime.Now.Year == b.start_time.Year && DateTime.Now.Month == b.start_time.Month && DateTime.Now.Day == b.start_time.Day)
            {
                if (tuple.Item2.id == b.charging_column.id)
                {
                    int index = 0;
                    foreach (Tuple<DateTime, DateTime> tuple1 in tuple.Item1)
                    {
                        if (tuple1.Item1 > b.end_time && tuple.Item1[index -1].Item1 < b.start_time)
                        {
                            tuple.Item2.list.Insert(index - 1, new Tuple<DateTime, DateTime>(b.start_time, b.end_time));
                            b.charging_column = tuple.Item2;
                            b.Accept();
                            return;
                        }
                    }
                }
            }
        }

        public static List<Booking> GenerateBookingSuggestion(List<Tuple<List<Tuple<DateTime,DateTime>>, ChargingColumn>> list, Booking b)
        {
            List<Booking> bookingsuggestions;
            List<Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn>> locationlist = LocationFilter(list, b.location);
            int index = 0;
            foreach(Tuple<List<Tuple<DateTime, DateTime>>, ChargingColumn> tuple in locationlist)
            {
                TimeSpan timetoend = ChargingTime.RealChargingTime(tuple.Item2.charging_column_type_id, b) + new TimeSpan(0,15,0);
                foreach (Tuple<DateTime,DateTime> tupleTime in tuple.Item1)
                {
                    DateTime currentEndTime = tupleTime.Item2;
                    DateTime nextStartTime = tuple.Item1[index + 1].Item1;
                    if(currentEndTime < b.start_time && b.start_time + timetoend < nextStartTime)
                    {
                        bookingssuggestions.Add(new Booking())
                    }
                }
                ++index;
            }

            return bookingsuggestions;
        }

    }
}
