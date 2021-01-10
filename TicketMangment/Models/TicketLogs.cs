using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class TicketLogs
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserLog { get; set; }
        public string Message { get; set; }
        public DateTime LogDate { get; set; }
    }
}
