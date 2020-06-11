using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.OccupancyPlan;

namespace Uebungsprojekt.Simulation
{
    public class Simulation
    {
        private List<Object> history_metrics;
        private int tick;
        private BookingGenerator booking_generator;
        private OccupancyPlan.OccupancyPlan occupancy_plan;
        private int weeks;

        /// <summary>
        /// Constructor of Simulation
        /// </summary>
        /// <param name="tick_minutes">Length of one tick</param>
        /// <param name="weeks_to_simulate">How many weeks to simulate</param>
        public Simulation(int tick_minutes, List<Tuple<DayOfWeek, TimeSpan>> rush_hours, int min, int max, double spread, int weeks_to_simulate)
        {
            history_metrics = new List<Object>();
            tick = tick_minutes;
            booking_generator = new BookingGenerator(new TimeSpan(0, tick_minutes, 0), rush_hours, min, max, spread, new List<Object>(), weeks_to_simulate);
            occupancy_plan = OccupancyPlan.OccupancyPlan.GetNewOccupancyPlan();
        }

        /// <summary>
        /// Start simulation
        /// </summary>
        public void RunSimulation()
        {
            var random = new Random();
            foreach (IEnumerable<Booking> bookings in booking_generator.Generate())
            {
                foreach (Booking booking in bookings)
                {
                    if (!occupancy_plan.AcceptBooking(booking))
                    {
                        IEnumerable<Booking> suggested_bookings = occupancy_plan.GenerateBookingSuggestions(booking);
                        int index = random.Next(suggested_bookings.Count());
                        occupancy_plan.AcceptBooking(suggested_bookings.ElementAt(index));
                    }
                }
                history_metrics.Add(GetCurrentWorkload());
            }
        }

        /// <summary>
        /// Get the current workload of each of the assigned charging zones
        /// </summary>
        /// <returns></returns>
        public double GetCurrentWorkload()
        {
            return 0.5;
        }
    }
}
