using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uebungsprojekt.Models
{
    public class SimulationConfig
    {
        public SimulationConfig()
        {
            rush_hours = new List<Tuple<DayOfWeek, TimeSpan>>();
            vehicles = new List<Vehicle>();
        }
        public int id { get; set; }
        public int tick_minutes { get; set; }
        public List<Tuple<DayOfWeek, TimeSpan>> rush_hours { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public double spread { get; set; }
        public int weeks { get; set; }
        public List<Vehicle> vehicles { get; set; }
    }
}
