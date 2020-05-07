using System;
namespace Uebungsprojekt.Models
{
    public class Booking
    {
        public int charge { get; set; }
        public int needed_distance { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }

        public Booking()
        { 

        }
    }
}
