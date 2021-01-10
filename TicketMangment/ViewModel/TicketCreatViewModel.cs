using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class TicketCreatViewModel
    {
        public string Subject { get; set; }
        public string RequestDetail { get; set; }
        public string Location { get; set; }
        public int DepartmentId { get; set; }
        public int PriorityId { get; set; }
        public string AssignedTo { get; set; }
        public List<Priority> Priorities { get; set; }
        public List<Department> Departments { get; set; }
        public ApplicationUser User { get; set; }
        public string NoteBody { get; set; }
    }
}
