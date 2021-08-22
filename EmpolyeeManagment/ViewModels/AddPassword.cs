using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class AddPassword
    { 
        [Required]
        public string Id { get; set; } 
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The two Password didn't match")]
        public string ConfirmPassword { get; set; }
    }
}
