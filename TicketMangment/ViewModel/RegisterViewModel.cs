using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.ViewModel
{
    public class RegisterViewModel
    {

        // user properties
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }

        // company properties
        [Required]
        [Display (Name ="Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [Display (Name ="Company Email")]
        public string CompanyEmail { get; set; }

        [Display (Name ="Company Address")]
        public string Address { get; set; }

        [Required]
        [Display (Name ="Company Phone Number")]
        public string CompanyPhoneNumber { get; set; }
    }
}
