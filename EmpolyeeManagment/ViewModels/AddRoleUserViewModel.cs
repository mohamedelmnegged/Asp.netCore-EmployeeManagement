using EmpolyeeManagment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class AddRoleUserViewModel
    {
        public AddRoleUserViewModel()
        {
            Users = new List<ApplicationUser>();
        }
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string UserId { get; set; }

        public List<ApplicationUser>  Users { get; set; }
    }
}
