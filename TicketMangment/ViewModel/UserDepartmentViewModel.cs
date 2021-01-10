using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.ViewModel
{
    public class UserDepartmentViewModel
    {
        public UserDepartmentViewModel()
        {
            Users = new List<string>();
        }
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public List<string> Users { get; set; }
    }
}
