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
        private OccupancyPlan()
        {

        }

        public static OccupancyPlan GetOccupancyPlan()
        {
            if (occupancy_plan == null)
                occupancy_plan = new OccupancyPlan();
            return occupancy_plan;
        }

        public static OccupancyPlan GetNewOccupancyPlan()
        {
            return new OccupancyPlan();
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
