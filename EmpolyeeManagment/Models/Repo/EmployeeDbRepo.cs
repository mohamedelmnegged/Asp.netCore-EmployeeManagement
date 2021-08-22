using EmpolyeeManagment.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using EmpolyeeManagment.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace EmpolyeeManagment.Models.Repo
{
    public class EmployeeDbRepo : IEmployeeCollection<Employee>
    {
        private readonly AppDbContext context;

        public EmployeeDbRepo(AppDbContext context)
        {
            this.context = context;
        }
        public Employee Find(int id)
        {
            return this.context.employees.SingleOrDefault(g => g.Id == id); 
        } 
        public IEnumerable<Employee> GetAll()
        {
            return this.context.employees; 
        } 

        public Employee Add(Employee employee)
        {
         
            employee.Id = this.context.employees.Count() + 1;
            this.context.employees.Add(employee);
            this.context.SaveChanges(); 
            return employee; 
        }

        public void Delete(int id)
        {
            Employee SearchedEmployee = Find(id); 
            if(SearchedEmployee != null)
            {
                this.context.employees.Remove(SearchedEmployee);
                this.context.SaveChanges();
            } 
        }

        public Employee Update(Employee entity)
        {
            var employee = this.context.employees.Attach(entity);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified; 
            this.context.SaveChanges();
            return entity; 
        } 

       
    }
}
