using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Booking
    {
        public int SoC { get; set; }

        public int reqDistance { get; set; }

        public DateTime start { get; set; }

        public DateTime end { get; set; }
        
        public Booking()
        {

        }
    }
}
