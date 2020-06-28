using System;
using System.Collections.Generic;

namespace Uebungsprojekt.Models
{
    public class SimulationResult
    {
        public int id { get; set; }
        public SimulationConfig config { get; set; }
        public SimulationInfrastructure infrastructure { get; set; }
        public List<double> total_workload { get; set; }
        public List<int> num_generated_bookings { get; set; }
        public List<int> num_unsatisfiable_bookings { get; set; }
        public bool done { get; set; }
        public List<Tuple<Booking, Booking>> unsatisfiable_bookings_with_suggestion { get; set; }
        
        public SimulationResult(SimulationConfig config, SimulationInfrastructure infrastructure)
        {
            this.config = config;
            this.infrastructure = infrastructure;

            total_workload = new List<double>();
            num_generated_bookings = new List<int>();
            num_unsatisfiable_bookings = new List<int>();
            
            unsatisfiable_bookings_with_suggestion = new List<Tuple<Booking, Booking>>();
        }
    }
}