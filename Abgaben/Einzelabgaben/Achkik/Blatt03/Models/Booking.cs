using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softwareprojekt.Models
{
    public class Booking
    {
        public double SoC { get; set; }
        public int NeededDistance { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; } 
    }
}
