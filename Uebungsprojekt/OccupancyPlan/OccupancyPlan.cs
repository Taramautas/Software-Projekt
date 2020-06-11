using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.OccupancyPlan
{
    public class OccupancyPlan
    {
        private static OccupancyPlan occupancy_plan;

        public List<Object> locations; // TODO: Change to Location type
        private OccupancyPlan(List<Object> location_list)
        {
            locations = location_list;
        }

        public static OccupancyPlan GetOccupancyPlan(List<Object> locations)
        {
            if (occupancy_plan == null)
                occupancy_plan = new OccupancyPlan(locations);
            return occupancy_plan;
        }

        public static OccupancyPlan GetNewOccupancyPlan(List<Object> locations)
        {
            return new OccupancyPlan(locations);
        }
        public bool AcceptBooking(Booking booking)
        {
            return true;
        }

        public IEnumerable<Booking> GenerateBookingSuggestions(Booking booking)
        {
            return new List<Booking>();
        }
    }
}
