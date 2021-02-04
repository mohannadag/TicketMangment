using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.ViewModel
{
    public class RoleClaimsViewModel
    {
        public RoleClaimsViewModel()
        {
            Claims = new List<RoleClaim>();
        }
        public string RoleId { get; set; }
        public List<RoleClaim> Claims { get; set; }
    }
}
