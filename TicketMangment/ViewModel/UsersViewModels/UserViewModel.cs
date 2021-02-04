using Microsoft.AspNetCore.Http;
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
    public class UserViewModel
    {
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
        
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }
        
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        public string ExistingPhotoPath { get; set; }
        public IFormFile Photo { get; set; }
        public string Department { get; set; }
        //[ForeignKey("DepartmentId")]
        //public Department Department { get; set; }
    }
}
