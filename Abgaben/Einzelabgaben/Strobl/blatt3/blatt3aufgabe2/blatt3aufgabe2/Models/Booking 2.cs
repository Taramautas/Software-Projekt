using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blatt3aufgabe2.Models
{
    public class Booking
    {
        public int ladestand { get; set; }
        public int fahrstrecke { get; set; }
        public DateTime startzeit { get; set; }
        public DateTime endzeit { get; set; }
        
        public Booking()
        {

        }
    }
}
