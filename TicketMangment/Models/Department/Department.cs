using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class Department
    {
        public Department()
        {
            ListOfUsersName = new List<string>();
        }
        public int DepartmentId { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name ="Department Name")]
        public string DepartmentName { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public RecordStatus? RecordStatus { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        [NotMapped]
        public List<string> ListOfUsersName { get; set; }
        public string DepAdmin { get; set; }
        public int CompanyId { get; set; }
    }
}
