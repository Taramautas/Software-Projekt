using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class SimulationConfig
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "Please enter the number of minutes associated with each tick.")]
        [Range(1, 100)]
        public int tick_minutes { get; set; }
        
        [Required(ErrorMessage = "Please specify the rush hours.")]
        public List<Tuple<DayOfWeek, TimeSpan>> rush_hours { get; set; }
        
        [Required(ErrorMessage = "Please enter the minimum number of generated bookings per tick.")]
        [Range(1, 100)]
        public int min { get; set; }
        
        [Required(ErrorMessage = "Please enter the maximum number of generated bookings per tick.")]
        [Range(1, 100)]
        public int max { get; set; }
        
        [Required(ErrorMessage = "Please enter the spread of the bookings over time.")]
        [Range(1, 100)]
        public double spread { get; set; }
        
        [Required(ErrorMessage = "Please enter the number of weeks to simulate.")]
        [Range(1, 100)]
        public int weeks { get; set; }
        
        [Required(ErrorMessage = "Please specify a list of vehicles to randomly sample from while generating bookings (State of charge is also chosen randomly).")]
        public List<Vehicle> vehicles { get; set; }
        
        public SimulationConfig()
        {
            rush_hours = new List<Tuple<DayOfWeek, TimeSpan>>();
            vehicles = new List<Vehicle>();
        }
    }
}
