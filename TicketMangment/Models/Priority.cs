using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class Priority
    {
        public int PriorityId { get; set; }
        [Required]
        public string PriorityName { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public RecordStatus? RecordStatus { get; set; }
    }
}
