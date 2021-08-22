using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            UserRoles = new List<UserRole>(); 
        }
        public string UserId { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }
}
