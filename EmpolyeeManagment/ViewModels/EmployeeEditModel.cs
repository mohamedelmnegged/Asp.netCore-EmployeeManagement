using EmpolyeeManagment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.ViewModels
{
    public class EmployeeEditModel
    {
        public Employee oldEmployee { get; set; } 

        public IFormFile NewImage { get; set; }

    }
}
