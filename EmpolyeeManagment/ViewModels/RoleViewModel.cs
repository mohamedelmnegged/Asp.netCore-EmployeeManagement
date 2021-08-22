using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class RoleViewModel
    {
        
        public string Id { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage ="the length you enterd is not valid")]
     //   [Remote(action: "CheckRoleNameExist", controller:"adminstration")]
        public string Name { get; set; } 

        public string  Concurrency { get; set; }
    }
}
