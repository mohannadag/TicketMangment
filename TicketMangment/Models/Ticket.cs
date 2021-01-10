using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketMangment.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string RequestDetail { get; set; }
        public string Location { get; set; }
        [Display(Name ="Ticket status")]
        public TicketStatus TicketStatus { get; set; }
        [Display(Name ="Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        [Display(Name ="Priority")]
        public int PriorityId { get; set; }
        public Priority Priority { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }

        // public TicketDetail TicketDetail { get; set; }

        public IEnumerable<Note> Notes { get; set; }
        public IEnumerable<TicketLogs> TicketLogs { get; set; }

        // which user will take the ticket and solve it
        [Display(Name ="Assign Ticket to")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public ApplicationUser AssignedUser { get; set; }

        // I have to move those properties to TicketDetails Model
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Photo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public RecordStatus? RecordStatus { get; set; }
    }

    public enum TicketStatus
    {
        Open = 1,
        Closed = 2,
        Solved = 3,
        Asssigned = 4,
        OnHold = 5,
        ReOpened = 6,
        InProgress = 7
    }

    public enum RecordStatus
    {
        notdeleted = 0,
        deleted = 1
    }
}
