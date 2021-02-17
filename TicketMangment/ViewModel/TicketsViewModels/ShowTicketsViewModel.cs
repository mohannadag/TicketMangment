using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class ShowTicketsViewModel
    {
        public int TicketId { get; set; }
        public string Subject { get; set; }
        public string RequestDetail { get; set; }
        [Display(Name = "Ticket status")]
        public TicketStatus TicketStatus { get; set; }
        public string Priority { get; set; }
        public IEnumerable<Note> Notes { get; set; }
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
