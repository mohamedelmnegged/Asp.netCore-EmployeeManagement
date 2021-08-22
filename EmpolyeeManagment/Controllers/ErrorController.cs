using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{status}")]
        public ViewResult HttpRedirectRequestError(int status)
        {
            ViewBag.ErrorMessage = status switch
            {
                404 => "the Page is not found",
                _ => " there are an error ",
            };
            return View("~/Views/CustomError.cshtml"); 
                  
        }
    }
}
