using EmpolyeeManagment.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using EmpolyeeManagment.Enums;

namespace EmpolyeeManagment.Models.Repo
{
    public class EmpolyeeRepo : IEmployeeCollection<Employee>
    {
        private readonly List<Employee> employee = new () ;

        public EmpolyeeRepo()
        {
            this.employee.Add(new Employee {Id = 1, Name= "Mohamed", Email = "Mohamed@example.com", Department =Dep.IT }); 
            this.employee.Add(new Employee {Id = 2, Name= "May", Email = "May@example.com", Department=Dep.HR }); 
            this.employee.Add(new Employee {Id = 3, Name= "Omar", Email = "Omar@example.com", Department=Dep.IT }); 
        }
        public Employee Find(int id)
        {
            return this.employee.SingleOrDefault(g => g.Id == id); 
        } 
        public IEnumerable<Employee> GetAll()
        {
            return this.employee; 
        } 

        public Employee Add(Employee employee)
        {
            this.employee.Add(employee);
            return employee; 
        }

        public void Delete(int id)
        {
            Employee SearchedEmployee = Find(id); 
            if(SearchedEmployee != null)
            {
                this.employee.Remove(SearchedEmployee); 
            } 
        }

        public Employee Update(Employee entity)
        {
            Employee SearchEmployee = Find(entity.Id); 
            if(SearchEmployee != null)
            {
                SearchEmployee.Name = entity.Name;
                SearchEmployee.Email = entity.Email;
                SearchEmployee.Department = entity.Department; 
            }
            return SearchEmployee; 
        }
    }
}
