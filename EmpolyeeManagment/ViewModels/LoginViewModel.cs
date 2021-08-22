using EmpolyeeManagment.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace EmpolyeeManagment.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Password(check:"123", ErrorMessage ="the Password you enter is very easy")]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public List<AuthenticationScheme> ExternalProvider{ get; set; }
    }
}
