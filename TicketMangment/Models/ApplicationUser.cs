using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PhotoPath { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }

    public enum InDepRole
    {
        None = 0,
        Admin = 1,
        Employee = 2
    }

}