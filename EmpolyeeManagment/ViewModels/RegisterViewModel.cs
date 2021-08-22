using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmpolyeeManagment.CustomValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmpolyeeManagment.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Remote(action: "IsUserNameExist", controller:"Account")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailExist", controller:"Account")]
        [EmailDomain(allowedEmail:"medo.com", ErrorMessage =" Your Email is not match the accepted email")]
        public string Email { get; set; }

        [Required]
        [Password(check: "123", ErrorMessage = "sorry but this message is very easy")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confrim Password")]
        [Compare("Password", ErrorMessage ="The two Passwords Didn't match each other")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }
        public string Department { get; set; }
    }
}
