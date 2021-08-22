using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.CustomValidation
{
    public class Password : ValidationAttribute
    {
        private readonly string check;

        public Password(string check)
        {
            this.check = check;
        }

        public override bool IsValid(object value)
        {
            string ch = value.ToString();
            if (ch == this.check)
                return false;
            return true; 
        }
    }
}
