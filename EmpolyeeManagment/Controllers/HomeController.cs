using EmpolyeeManagment.Enums;
using EmpolyeeManagment.Models;
using EmpolyeeManagment.Models.Interfaces;
using EmpolyeeManagment.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmpolyeeManagment.Models.Repo;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;

namespace EmpolyeeManagment.Controllers
{
    
    public class HomeController : Controller
    {
        private  readonly IEmployeeCollection<Employee> _employee;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly AppDbContext context;

        public HomeController(IEmployeeCollection<Employee> employee , IWebHostEnvironment hostingEnvironment, AppDbContext _context)
        {
            this._employee = employee;
            this.hostingEnvironment = hostingEnvironment;
            context = _context;
        }
        public ActionResult Index()
        {
            var model = _employee.GetAll();
            ViewData["title"] = "Index Page";
            return View(model); 
        } 
        public ViewResult Details(int id)
        {
           var model = _employee.Find(id);

            return View(model);
        } 
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
         //   var model = new EmployeeViewModel {Id =  _employee.GetAll().Count(f => f.Id == f.Id) + 1 }; 

            return View(); 
        }

        [HttpPost]
        [Authorize]
        public  ActionResult Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            { 
                if(model.Department == Dep.None)
                {
                    ModelState.AddModelError("Department", "Please choose a department from the select list");
                    return View();
                }
                // make edit upload the image 
                string imageName =   UploadImage(model.Photo); 
                
                //then add the employee 
                if(imageName != "")
                {
                    var employee = new Employee {  
                        Name = model.Name, 
                        Email = model.Email, 
                        Department = model.Department, 
                        ImagePath = imageName
                    };

                    _employee.Add(employee);
                    return RedirectToAction("details", new { employee.Id }); 
                }
                
            }
            return View(); 
        } 

        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var employee = _employee.Find(id); 
            if(employee != null)
                return View(employee);

            return View("NotFoundId", id); 

        } 

        [HttpPost]
        [Authorize]
        public ActionResult Delete(string image, int id)
        {
            //first delete the image related to the user then delete it
            DeleteImage(image); 

            //then delete   the employee   
            this._employee.Delete(id);
            return RedirectToAction("index"); 
        } 

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var oldEmployee =  _employee.Find(id);
            if (oldEmployee != null)
            {
                var model = new EmployeeEditModel { oldEmployee = oldEmployee};
                return View(model);
            }
            
            return View("NotFoundId", id); 
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(EmployeeEditModel newEmployee)
        {
            if (ModelState.IsValid)
            {
                // first check if the employee add valid department 
                if (newEmployee.oldEmployee.Department == Dep.None)
                {
                    ModelState.AddModelError("", "Please Choose valid Department"); 
                    return View(newEmployee);
                } 

                //second check if the user upload a new image 
                if(newEmployee.NewImage != null )
                {
                    //upload the image  
                   string NewImage =  UploadImage(newEmployee.NewImage);
                    //then delete the old one 
                    DeleteImage(newEmployee.oldEmployee.ImagePath);

                    newEmployee.oldEmployee.ImagePath = NewImage; 
                }

                //here the update 
                _employee.Update(newEmployee.oldEmployee);
                return View("details", newEmployee.oldEmployee); 
            }
            ModelState.AddModelError("", "Please Update valid data"); 
            return View(newEmployee); 
        }

        public string UploadImage(IFormFile photo)
        {
            string imageName = "";
            if (photo != null)
            {
                string path = Path.Combine(hostingEnvironment.WebRootPath, "images");
                imageName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                string imagePath = Path.Combine(path, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }
            }
           

            return imageName; 
        } 
        public void DeleteImage(string image)
        {
            if (image != null)
            {
                image = Path.Combine("images", image);
                string imageUrl = Path.Combine(hostingEnvironment.WebRootPath, image);
                //  return hostingEnvironment.WebRootPath; 
                if (System.IO.File.Exists(imageUrl))
                    System.IO.File.Delete(imageUrl);
            }
        } 

      public string Test()
        {
            DbConnection conn = context.Database.GetDbConnection();
            conn.Open();
            var a = conn.CreateCommand();
            a.CommandText = "Select * from employees";
            var data = a.ExecuteReader(); 
            conn.Close();
            var d = ""; 
            while (data.Read())
            {
                d += data.GetString(0) + " ";
            }
            return d;
        }
    }
}
