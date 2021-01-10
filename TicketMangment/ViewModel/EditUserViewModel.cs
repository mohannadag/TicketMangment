using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class EditUserViewModel
    {
        //public EditUserViewModel()
        //{
        //    Roles = new List<string>();
        //    Claims = new List<string>();
        //}
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public List<string> Claims { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        //public InDepRole? InDepRole { get; set; }
    }
}
