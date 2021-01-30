using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.ViewModel
{
    public class TicketsViewModel
    {
        public IEnumerable<ShowTicketsViewModel> DepartmentTickets { get; set; }
        public IEnumerable<ShowTicketsViewModel> MyTickets { get; set; }
    }
}
