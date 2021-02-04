using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class TicketDetail
    {
        public int TicketDetailId { get; set; }
        public int TicketId { get; set; }
        public int NoteId { get; set; }
        public List<Note> Notes { get; set; }
        public string Attachment { get; set; }
        public RecordStatus? RecordStatus { get; set; }

        //created by
        public string CreatedBy { get; set; }
        public ApplicationUser User { get; set; }

        //Modifiedby users
        public string ModifiedBy { get; set; }
        public List<ApplicationUser> Users { get; set; }

        //Dates of creation and modification
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
