using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.SharedClasses
{
    public class ChartClass
    {
        public string[] Labels { get; set; }
        public int[] SolvedTicketsValues { get; set; }
        public int[] AllTicketsValues { get; set; }
    }

    public class Chart2Class
    {
        public string[] Labels { get; set; }
        public int[] TicketsCount { get; set; }
    }
}
