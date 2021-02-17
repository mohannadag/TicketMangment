using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class TicketCreatViewModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Request Details")]
        public string RequestDetail { get; set; }
        public string Location { get; set; }
        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Required]
        [Display(Name = "Priority")]
        public int PriorityId { get; set; }
        [Display(Name = "Assign To")]
        public string AssignedTo { get; set; }
        public List<Priority> Priorities { get; set; }
        public List<Department> Departments { get; set; }
        public ApplicationUser User { get; set; }
        [Display(Name = "Note")]
        public string NoteBody { get; set; }
    }
}
