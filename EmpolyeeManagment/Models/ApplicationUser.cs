using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
        public string Department { get; set; }
    }
}
