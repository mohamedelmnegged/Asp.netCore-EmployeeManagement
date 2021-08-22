using EmpolyeeManagment.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace EmpolyeeManagment.ViewModels
{
    public class EmployeeViewModel
    {
        [Required]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public Dep Department { get; set; }
        [Required]
        public IFormFile Photo { get; set; } 
    }
}
