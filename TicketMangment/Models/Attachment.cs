using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string ServerFileName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
