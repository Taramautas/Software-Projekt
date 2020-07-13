using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.OccupancyPlans;

namespace Uebungsprojekt.Simulations
{
    public class Simulation
    {
        public SimulationResult simulation_result;
        private BookingGenerator booking_generator;
        private OccupancyPlan occupancy_plan;
        
        /// <summary>
        /// Setup
        /// </summary>
        /// <param name="config">Simulation Configuration</param>
        /// <param name="infrastructure">Simulation Infrastructure</param>
        public Simulation(SimulationConfig config, SimulationInfrastructure infrastructure, SimulationResult simulation_result)
        {
            int booking_dao_id = BookingDaoImpl.CreateNewDaoId();
            this.simulation_result = simulation_result;
            booking_generator = new BookingGenerator(config);
            occupancy_plan = new OccupancyPlan(
                infrastructure.location_dao_id, 
                infrastructure.charging_zone_dao_id, 
                infrastructure.charging_column_dao_id, 
                booking_dao_id
                );
        }

        /// <summary>
        /// Run the simulation and fill results
        /// </summary>
        /// <returns>Boolean indicating whether the simulation ran the first time or not</returns>
        public bool RunSimulation()
        {
            // Don't run simulation twice
            if (simulation_result.done)
                return false;
            
            var random = new Random();
            // Iterate through each tick (list of bookings) returned by booking generator
            foreach (IEnumerable<Booking> bookings in booking_generator.Generate())
            {
                int num_unsatisfiable_bookings = 0;
                // Try to accept all generated bookings
                foreach (Booking booking in bookings)
                {
                    // If the booking could not be accepted, get possible alternatives and log to results
                    if (!occupancy_plan.AcceptBooking(booking))
                    {
                        num_unsatisfiable_bookings++;
                        IEnumerable<Booking> suggested_bookings = occupancy_plan.GenerateBookingSuggestions(booking);
                        int index = random.Next(suggested_bookings.Count());
                        occupancy_plan.AcceptBooking(suggested_bookings.ElementAt(index));
                        simulation_result.unsatisfiable_bookings_with_suggestion.Add(new Tuple<Booking, Booking>(booking, suggested_bookings.ElementAt(index)));
                    }
                }
                // Update results after each tick
                simulation_result.num_generated_bookings.Add(bookings.Count());
                simulation_result.num_unsatisfiable_bookings.Add(num_unsatisfiable_bookings);
                simulation_result.total_workload.Add(occupancy_plan.GetCurrentWorkload());
            }
            return true;
        }
    }
}
