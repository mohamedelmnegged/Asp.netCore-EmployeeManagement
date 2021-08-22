using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.CustomValidation
{
    public class EmailDomain : ValidationAttribute
    {
        private readonly string allowedEmail;

        public EmailDomain(string allowedEmail)
        {
            this.allowedEmail = allowedEmail;
        }

        public override bool IsValid(object value)
        {
            string[] values = value.ToString().Trim().Split('@');
            if (values[1].ToLower() == allowedEmail.ToLower())
                return true;
            return false; 
        } 
    }
}
