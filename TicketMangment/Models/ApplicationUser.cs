using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class ApplicationUser : IdentityUser
    {
        // TODO : create DisplayName and change it with username and make the username = email everywhere in the project
        [Required]
        [Display(Name = "Name")]
        public string DisplayName { get; set; }
        public string PhotoPath { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public int CompanyId { get; set; }
    }

    

}