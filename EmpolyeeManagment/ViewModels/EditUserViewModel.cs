using EmpolyeeManagment.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; 

namespace EmpolyeeManagment.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
       // [EmailDomain(allowedEmail: "medo.com", ErrorMessage = "your Email didn't match the required Email")]
        [Remote(action:"CheckEmailDomain", controller:"adminstration", ErrorMessage ="Your Email didn't match the required Email Domain")]
        public string Email { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string City { get; set; }
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6)]
        public string Password { get; set; }
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
