using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class EditTicketViewModel
    {
        public int TicketId { get; set; }
        public string Subject { get; set; }
        public string RequestDetail { get; set; }
        public string Location { get; set; }
        public int DepartmentId { get; set; }
        public int PriorityId { get; set; }
        public string AssignedTo { get; set; }
        public List<Priority> Priorities { get; set; }
        public Department Department { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string NoteBody { get; set; }
        public IEnumerable<Note> Notes { get; set; }
        public IEnumerable<TicketLogs> TicketLogs { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
