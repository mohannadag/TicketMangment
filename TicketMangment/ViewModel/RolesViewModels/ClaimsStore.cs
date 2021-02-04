using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TicketMangment.ViewModel
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Roles Mangment"),
            new Claim("Edit Role", "Roles Mangment"),
            new Claim("Delete Role", "Roles Mangment"),
            new Claim("Manage Claims Role", "Roles Mangment"),
            new Claim("List Roles", "Roles Mangment"),
            new Claim("Edit Users In Role", "Roles Mangment"),
            new Claim("Manage Claims User", "User Mangment"),
            new Claim("Users List", "User Mangment"),
            new Claim("Edit User", "User Mangment"),
            new Claim("Mange User Roles", "User Mangment"),
            new Claim("Delete User", "User Mangment"),
            new Claim("Access Control Panel", "Access Control Panel")

            //new Claim("Delete User", "Delete User")
        };
    }
}
