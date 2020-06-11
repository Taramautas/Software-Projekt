using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Simulation
{
    public class BookingGenerator
    {
        private TimeSpan start_time;
        private TimeSpan end_time;
        private int ticks_per_day;

        private TimeSpan tick;
        private int rush_hour_ticks;
        private int min;
        private int max;
        private double spread;
        private List<Object> vehicles; // FIXME: Adjust type when class is defined
        private int weeks;
        private int step;

        /// <summary>
        /// Constructor for BookingGenerator
        /// </summary>
        /// <param name="tick_minutes">Length of one tick</param>
        /// <param name="rush_hours">Rush hours as Tuple<DayOfWeek, TimeSpan></param>
        /// <param name="min_bookings">Minimum amount of bookings per tick</param>
        /// <param name="max_bookings">Maximum amount of bookings per tick</param>
        /// <param name="stochastic_spread">Spread</param>
        /// <param name="vehicles_list">List of vehicle objects to randomly select from</param>
        /// <param name="weeks_to_simulate">How many weeks to simualate</param>
        public BookingGenerator(SimulationConfig config)
        {
            start_time = new TimeSpan(6, 0, 0);
            end_time = new TimeSpan(18, 0, 0);

            tick = new TimeSpan(0, config.tick_minutes, 0);
            ticks_per_day = (int)((end_time - start_time) / tick);
            foreach (Tuple<DayOfWeek, TimeSpan> rush_hour in config.rush_hours)
            {
                int rush_hour_tick = (int)(rush_hour.Item1) * ticks_per_day + (int)((rush_hour.Item2 - start_time) / tick);
            }
            min = config.min;
            max = config.max;
            spread = config.spread;
            vehicles = config.vehicles;

            step = 0;
        }

        /// <summary>
        /// Generator function returning an iterable list of bookings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEnumerable<Booking>> Generate()
        {
            var random = new Random();
            // Simlation starts on Monday 06:00 and ends on Friday 18:00 (same times on all weekdays)
            foreach (int week in Enumerable.Range(1, weeks))
            {
                foreach (int day in Enumerable.Range(1, 5))
                {
                    foreach (int tick in Enumerable.Range(1, ticks_per_day))
                    {
                        // Get a propability score for recieving bookings while this tick
                        int new_bookings = (int)(getPropabilityScore() * max);
                        new_bookings = Math.Max(new_bookings, min);
                        List<Booking> newest_bookings = new List<Booking>();

                        foreach (int booking in Enumerable.Range(0, new_bookings))
                        {
                            int index = random.Next(vehicles.Count);
                            newest_bookings.Add(new Booking
                            {
                                
                            });
                        }
                        step++;
                        yield return newest_bookings;
                    }
                }
                step = 0;
            }
        }

        /// <summary>
        /// Get the propability score according to the distance to rush hours
        /// </summary>
        /// <returns></returns>
        private float getPropabilityScore()
        {
            return 0.5f;
        }
    }
}
