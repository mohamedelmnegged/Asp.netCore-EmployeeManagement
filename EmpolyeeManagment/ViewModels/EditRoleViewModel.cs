using EmpolyeeManagment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>(); 
        }
        public string Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public string Concurrency { get; set; }

        public IList<string>  Users { get; set; }
    }
}
