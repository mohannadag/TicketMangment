using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class Note
    {
        public int NoteId { get; set; }
        public int TicketId { get; set; }
        public string NoteBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Attachment { get; set; }
        public int WorkTime { get; set; }
        public string BillingRate { get; set; }
        public bool IsVisibleToClient { get; set; }
        public bool IsSolution { get; set; }
        public string CreatedBy { get; set; }
        public RecordStatus? RecordStatus { get; set; }
    }
}
