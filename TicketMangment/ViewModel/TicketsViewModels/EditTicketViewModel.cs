using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class EditTicketViewModel
    {
        public int TicketId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Request Details")]
        public string RequestDetail { get; set; }
        public string Location { get; set; }
        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Required]
        [Display(Name = "Priority")]
        public int PriorityId { get; set; }
        [Display(Name = "Assign To")]
        public string AssignedTo { get; set; }
        public List<Priority> Priorities { get; set; }
        public Department Department { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string NoteBody { get; set; }
        public IEnumerable<Note> Notes { get; set; }
        public IEnumerable<TicketLogs> TicketLogs { get; set; }
        [Display(Name = "Ticket Status")]
        public TicketStatus TicketStatus { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
