using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>() {

            new Claim("Create User", "Create User"), 
            new Claim("Update User", "Update User"),
            new Claim("Delete User", "Delete User"), 
            new Claim("Make Changes", "Make Changes"), 
        };
    }
}
