using System;
using System.Collections.Generic;

namespace Uebungsprojekt.Models
{
    public class SimulationResult
    {
        public int id { get; set; }
        public SimulationConfig config { get; set; }
        public SimulationInfrastructure infrastructure { get; set; }
        public List<Dictionary<int, double>> total_workload { get; set; }
        public List<int> num_generated_bookings { get; set; }
        public List<int> num_unsatisfiable_bookings { get; set; }
        public bool done { get; set; }
        
        public SimulationResult()
        {
            total_workload = new List<Dictionary<int, double>>();
            num_generated_bookings = new List<int>();
            num_unsatisfiable_bookings = new List<int>();
        }
    }
}