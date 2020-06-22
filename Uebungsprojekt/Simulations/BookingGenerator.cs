using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace Uebungsprojekt.Simulations
{
    public class BookingGenerator
    {
        public static TimeSpan start_time = new TimeSpan(6, 0, 0);
        public  static TimeSpan end_time = new TimeSpan(18, 0, 0);
        
        public static Random random = new Random();

        private TimeSpan tick_minutes;
        private int ticks_per_day;
        private List<int> rush_hour_ticks;

        private SimulationConfig config;

        private double max_probability;

        /// <summary>
        /// Constructor for BookingGenerator
        /// </summary>
        /// <param name="configuration">SimulationConfig</param>
        public BookingGenerator(SimulationConfig configuration)
        {
            config = configuration;
            tick_minutes = new TimeSpan(0, config.tick_minutes, 0);
            ticks_per_day = (int)((end_time - start_time) / tick_minutes);
            // Convert rush hours to ticks (for calculations)
            foreach (Tuple<DayOfWeek, TimeSpan> rush_hour in config.rush_hours)
            {
                int rush_hour_tick = (int)(rush_hour.Item1) * ticks_per_day + (int)((rush_hour.Item2 - start_time) / tick_minutes);
                rush_hour_ticks.Add(rush_hour_tick);
            }
            // Get maximum probability for given spread in order to normalize afterwards
            max_probability = (1 / (config.spread * Math.Sqrt(2 + Math.PI))) * Math.Pow(Math.E, (-0.5 * Math.Pow((0) / config.spread, 2)));
        }

        /// <summary>
        /// Generator function returning an iterable list of bookings
        /// </summary>
        /// <returns>List of List of Bookings for every tick</returns>
        public IEnumerable<IEnumerable<Booking>> Generate()
        {
            // Iterate through all weeks
            foreach (int week in Enumerable.Range(1, config.weeks))
            {
                // Iterate through all weekdays
                foreach (int day in Enumerable.Range(1, 5))
                {
                    // Iterate through all tick in a day
                    foreach (int tick in Enumerable.Range(1, ticks_per_day))
                    {
                        // For each tick, populate the list of new bookings
                        List<Booking> newest_bookings = new List<Booking>();
                        foreach (int booking in Enumerable.Range(0, GetNumberOfBookings(tick)))
                        {
                            int index = random.Next(config.vehicles.Count);
                            Object vehicle = config.vehicles[index];
                            newest_bookings.Add(new Booking());
                            // TODO: Set booking parameters randomly and accordingly to the chosen vehicle
                        }
                        // Return new bookings
                        yield return newest_bookings;
                    }
                }
            }
        }

        /// <summary>
        /// Get th number of bookings to create at the given tick
        /// </summary>
        /// <param name="tick"></param>
        /// <returns>Number of bookings</returns>
        private int GetNumberOfBookings(int tick)
        {
            // Get probability for given tick
            double probability = GetProbabilityScore(tick) / max_probability;
            // Normalize to specified minimum and maximum of bookings and decide whether to round up or down
            int number_bookings = config.min + (int)Math.Ceiling(probability * (config.max - config.min));
            if (random.Next(100) > probability * 100)
                number_bookings -= 1;
            return number_bookings;
        }

        /// <summary>
        /// Get the maximum for this tick on all normal distribution (one for each rush hour)
        /// </summary>
        /// <param name="tick">X</param>
        /// <returns>Probability between 0 and 1</returns>
        private double GetProbabilityScore(int tick)
        {
            double probability = 0.0;
            double normal_probability = 0.0;
            // Iterate over all rush hours as mean values
            foreach (int rush_hour_tick in rush_hour_ticks)
            {
                // Calculate probability with: x = tick; mean = rush_hour_tick; standard deviation = spread
                normal_probability = (1 / (config.spread * Math.Sqrt(2 + Math.PI))) * Math.Pow(Math.E,
                    (-0.5 * Math.Pow((tick - rush_hour_tick) / config.spread, 2)));
                probability = Math.Max(probability, normal_probability);
            }
            // Return maximum of those probabilities normalized to 0 and 1
            return probability / max_probability;
        }
    }
}
