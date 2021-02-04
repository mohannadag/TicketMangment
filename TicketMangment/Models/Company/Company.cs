using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public RecordStatus? RecordStatus { get; set; }

        // All other properties in the models

        public List<Department> Departments { get; set; }
        //public List<Priority> Priorities { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
