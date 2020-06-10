using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Booking
    {
        public int SoC;
        public int Route;
        public DateTime StartOfCharge;
        public DateTime EndOfCharge;
       public Booking(int soc, int route, DateTime startofcharge, DateTime endofcharge)
        {
            SoC = soc;
            Route = route;
            StartOfCharge = startofcharge;
            EndOfCharge = endofcharge;

        }



    }
}
