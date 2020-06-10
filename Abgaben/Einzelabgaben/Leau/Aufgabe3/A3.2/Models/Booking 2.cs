using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A3._2.Models
{
    public class Booking
    {
        public int aktuellerLadestandDesFahrzeuges { get; set; }
        public int benoetigeFahrstrecke { get; set; }
        public DateTime startZeit { get; set; }
        public DateTime endZeit { get; set; }
    }
}
