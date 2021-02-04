using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class DeletedTicketsViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Priority> Priorities { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
