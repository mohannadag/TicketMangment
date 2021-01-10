using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMangment.Models;

namespace TicketMangment.ViewModel
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }

        // this properety i used just in department controller AddUserToDep method
        //public InDepRole? InDepRole { get; set; }
        public string DepAdmin { get; set; }
    }
}
