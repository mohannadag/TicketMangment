using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class MyTicketsViewModel
    {
        public MyTicketsViewModel()
        {
            MyTickets = new List<Ticket>();
            CreatedTickets = new List<Ticket>();
        }
        public IEnumerable<Ticket> MyTickets { get; set; }
        public IEnumerable<Ticket> CreatedTickets { get; set; }
    }
}
