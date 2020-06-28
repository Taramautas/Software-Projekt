using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.OccupancyPlans
{
    public class OccupancyPlan
    {
        private static OccupancyPlan main_occupancy_plan;

        private object location_dao;
        private object charging_zone_dao;
        private object charging_column_dao;
        private object booking_dao;
        private OccupancyPlan(object location_dao, object charging_zone_dao, object charging_column_dao, object booking_dao)
        {
            // TODO: This pattern is just one possible solution to deal with different contexts in terms of simulation and user infrastructure 
            this.location_dao = location_dao;
            this.charging_zone_dao = charging_zone_dao;
            this.charging_column_dao = charging_column_dao;
            this.booking_dao = booking_dao;
        }

        public static OccupancyPlan GetOccupancyPlan()
        {
            if (main_occupancy_plan == null)
            {
                object location_dao = new object();
                object charging_zone_dao = new object();
                object charging_column_dao = new object();
                object booking_dao = new object();
                object vehicle_dao = new object();
                main_occupancy_plan = new OccupancyPlan(
                    location_dao, 
                    charging_zone_dao, 
                    charging_column_dao, 
                    booking_dao
                    );

            }
            return main_occupancy_plan;
        }

        public static OccupancyPlan GetSimulationOccupancyPlan(SimulationInfrastructure infrastructure)
        {
            return new OccupancyPlan(
                infrastructure.location_dao, 
                infrastructure.charging_zone_dao, 
                infrastructure.charging_column_dao, 
                infrastructure.booking_dao
                );
        }
        public bool AcceptBooking(Booking booking)
        {
            return true;
        }

        public IEnumerable<Booking> GenerateBookingSuggestions(Booking booking)
        {
            return new List<Booking>();
        }
        
        public double GetCurrentWorkload()
        {
            return 0.5;
        }
    }
}
