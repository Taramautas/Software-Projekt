using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using Uebungsprojekt.Algorithm;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.OccupancyPlans;

namespace Uebungsprojekt.Simulations
{
    public class Simulation
    {
        public SimulationResult simulation_result;
        private SimulationConfig config;
        private OccupancyPlan occupancy_plan;

        public static TimeSpan start_time = new TimeSpan(6, 0, 0);
        public static TimeSpan end_time = new TimeSpan(18, 0, 0);

        public static Random random = new Random();

        private TimeSpan tick_minutes;
        private int ticks_per_day;
        private List<int> rush_hour_ticks;
        private double max_probability;

        private DateTime start_datetime;

        private IMemoryCache cache;

        private int booking_dao_id;
        
        /// <summary>
        /// Setup
        /// </summary>
        /// <param name="config">Simulation Configuration</param>
        /// <param name="infrastructure">Simulation Infrastructure</param>
        public Simulation(SimulationConfig config, SimulationInfrastructure infrastructure, SimulationResult simulation_result, IMemoryCache cache)
        {
            booking_dao_id = BookingDaoImpl.CreateNewDaoId();
            this.simulation_result = simulation_result;
            this.config = config;
            occupancy_plan = new OccupancyPlan(
                infrastructure.location_dao_id, 
                infrastructure.charging_zone_dao_id, 
                infrastructure.charging_column_dao_id, 
                booking_dao_id,
                cache
                );

            tick_minutes = new TimeSpan(0, config.tick_minutes, 0);
            ticks_per_day = (int)((end_time - start_time) / tick_minutes);
            rush_hour_ticks = new List<int>();
            // Convert rush hours to ticks (for calculations)
            foreach (Tuple<DayOfWeek, TimeSpan> rush_hour in config.rush_hours)
            {
                if (rush_hour.Item1 != DayOfWeek.Saturday && rush_hour.Item1 != DayOfWeek.Sunday)
                {
                    int rush_hour_tick = (int)(rush_hour.Item1) * ticks_per_day + (int)((rush_hour.Item2 - start_time) / tick_minutes);
                    rush_hour_ticks.Add(rush_hour_tick);
                }
            }
            // Get maximum probability for given spread in order to normalize afterwards
            max_probability = (1.0 / (config.spread * Math.Sqrt(2.0 * Math.PI))) * Math.Exp(-0.5 * 0);
            this.cache = cache;
        }

        /// <summary>
        /// Generator function returning an iterable list of bookings
        /// </summary>
        /// <returns>List of List of Bookings for every tick</returns>
        public bool Run()
        {

            // Don't run simulation twice
            if (simulation_result.done)
                return false;
            
            
            start_datetime = DateTime.Today;
            start_datetime = start_datetime.AddDays(1);
            while (start_datetime.DayOfWeek != DayOfWeek.Monday)
                start_datetime = start_datetime.AddDays(1);

            start_datetime = start_datetime.Add(start_time);
            simulation_result.start_datetime = start_datetime;
            UserDao user_dao = new UserDaoImpl(cache);
            User vip = user_dao.GetById(3);
            User guest = user_dao.GetById(4);
            User employee = user_dao.GetById(5);

            // Iterate through all weeks
            foreach (int week in Enumerable.Range(0, config.weeks))
            {
                // Iterate through all weekdays
                foreach (int day in Enumerable.Range(0, 5))
                {
                    DateTime start = new DateTime();
                    // Iterate through all tick in a day
                    foreach (int tick in Enumerable.Range(0, ticks_per_day))
                    {
                        int number_bookings = GetNumberOfBookings(tick + day * ticks_per_day);
                        foreach (int booking in Enumerable.Range(0, number_bookings))
                        {
                            User user;
                            switch(random.Next(5))
                            {
                                case 0:
                                    user = vip;
                                    break;
                                case 1:
                                    user = guest;
                                    break;
                                default:
                                    user = employee;
                                    break;
                            }
                            Vehicle vehicle = config.vehicles[random.Next(config.vehicles.Count)];
                            Location location = occupancy_plan.GetAllLocations()[random.Next(occupancy_plan.GetAllLocations().Count)];
                            int state_of_charge = random.Next(33);
                            int target_state_of_charge = state_of_charge + 33 + random.Next(33);

                            start = start_datetime.AddDays(week * 5 + day);
                            start = start.Add(tick_minutes * tick);
                            start = start.Add(new TimeSpan(0, random.Next(45), 0));

                            int hours = random.Next(1) + 1;
                            int minutes = random.Next(60);
                            
                            DateTime end = start.Add(new TimeSpan(hours, minutes, 0));

                            if (end.TimeOfDay > end_time)
                                break;
                                
                            occupancy_plan.AddNewBooking(new Booking
                            {
                                // Start state of charge is between 0 and 50
                                start_state_of_charge = state_of_charge,
                                // Target state of charge is between 25 and 50 more then start state of charge
                                target_state_of_charge = target_state_of_charge,
                                start_time = start,
                                end_time = end,
                                // Take a random vehicle from the given list
                                vehicle = vehicle,
                                // Takes randomly from VIP, Guest and Employee (1 : 1 : 3)
                                user = user,
                                // Random location
                                location = location,
                            });

                            Console.Out.WriteLine("Start soc: " + state_of_charge);
                            Console.Out.WriteLine("End soc: " + target_state_of_charge);
                            Console.Out.WriteLine("Start time: " + start);
                            Console.Out.WriteLine("End time: " + end);
                            Console.Out.WriteLine("Vehicle: " + vehicle.model_name);
                            Console.Out.WriteLine("Vehicle: " + vehicle.user.name);
                            Console.Out.WriteLine("Vehicle: " + vehicle.user.role);
                            Console.Out.WriteLine("User: " + user.name);
                            Console.Out.WriteLine("Location: " + location.city);
                        }
                        // Update results after each tick
                        simulation_result.num_generated_bookings.Add(number_bookings);
                    }
                    DistributionAlgorithm.DistributionAlg(
                        new ChargingColumnDaoImpl(cache), 
                        simulation_result.infrastructure.charging_column_dao_id,
                        new BookingDaoImpl(cache),
                        start,
                        booking_dao_id
                        );
                    foreach (int tick in Enumerable.Range(0, ticks_per_day))
                    {
                        DateTime time = start_datetime.AddDays(week * 5 + day).Add(tick_minutes * tick);
                        simulation_result.total_workload.Add(occupancy_plan.GetCurrentWorkload(time));
                    }
                }
            }
            simulation_result.done = true;
            return true;
        }

        /// <summary>
        /// Get th number of bookings to create at the given tick
        /// </summary>
        /// <param name="tick"></param>
        /// <returns>Number of bookings</returns>
        private int GetNumberOfBookings(int tick)
        {
            // Get probability for given tick
            double probability = GetProbabilityScore(tick);

            // Normalize to specified minimum and maximum of bookings and decide whether to round up or down
            int number_bookings = config.min + (int)(probability * (config.max - config.min));
            return Math.Min(number_bookings, config.max);
        }

        /// <summary>
        /// Get the maximum for this tick on all normal distribution (one for each rush hour)
        /// </summary>
        /// <param name="tick">X</param>
        /// <returns>Probability between 0 and 1</returns>
        private double GetProbabilityScore(int tick)
        {
            double probability = 0.0;
            double normal_probability;
            tick %= 5 * ticks_per_day;
            // Iterate over all rush hours as mean values
            foreach (int rush_hour_tick in rush_hour_ticks)
            {
                // Calculate probability with: x = tick; mean = rush_hour_tick; standard deviation = spread
                normal_probability = (1.0 / (config.spread * Math.Sqrt(2.0 * Math.PI))) * Math.Exp(-0.5 * Math.Pow((tick - rush_hour_tick) / config.spread, 2.0));
                probability = Math.Max(probability, normal_probability);
            }
            // Return maximum of those probabilities normalized to 0 and 1
            return probability / max_probability;
        }
    }
}
