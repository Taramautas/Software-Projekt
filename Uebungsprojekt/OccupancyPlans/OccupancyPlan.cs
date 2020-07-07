using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.OccupancyPlans
{
    public class OccupancyPlan
    {
        private static OccupancyPlan main_occupancy_plan;

        private int location_dao_id;
        private int charging_zone_dao_id;
        private int charging_column_dao_id;
        private int booking_dao_id;
        public OccupancyPlan(int location_dao_id, int charging_zone_dao_id, int charging_column_dao_id, int booking_dao_id)
        {
            this.location_dao_id = location_dao_id;
            this.charging_zone_dao_id = charging_zone_dao_id;
            this.charging_column_dao_id = charging_column_dao_id;
            this.booking_dao_id = booking_dao_id;
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
