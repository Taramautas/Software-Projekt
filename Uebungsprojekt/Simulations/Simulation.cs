using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using Uebungsprojekt.OccupancyPlans;

namespace Uebungsprojekt.Simulations
{
    public class Simulation
    {
        private List<Object> history_metrics;
        private int tick;
        private BookingGenerator booking_generator;
        private OccupancyPlan occupancy_plan;

        /// <summary>
        /// Constructor of Simulation
        /// </summary>
        /// <param name="config">Simulation parameters</param>
        /// <param name="locations">List of Location objects; TODO: Change type</param>
        public Simulation(SimulationConfig config, List<Object> locations)
        {
            history_metrics = new List<Object>();
            tick = config.tick_minutes;
            booking_generator = new BookingGenerator(config);
            occupancy_plan = OccupancyPlan.GetNewOccupancyPlan(locations);
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
