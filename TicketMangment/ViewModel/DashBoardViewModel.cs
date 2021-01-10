using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class DashBoardViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Ticket> PreviousMonthTickets { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}
