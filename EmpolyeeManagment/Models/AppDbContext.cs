using EmpolyeeManagment.Enums;
using EmpolyeeManagment.Models.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Models
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    
        public DbSet<Employee> employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1, 
                    Name = "Mohamd", 
                    Email = "Mohamed@example.com", 
                    Department = Dep.IT
                }, 
                new Employee
                {
                    Id = 2, 
                    Name = "Omar", 
                    Email = "Omar@example.com", 
                    Department = Dep.IT
                }, 
                new Employee
                {
                    Id = 3, 
                    Name = "May", 
                    Email = "May@example.com", 
                    Department = Dep.HR
                }
                );
            
            //modelBuilder.Entity<Employee>(entity =>
            //    {
            //        entity.HasNoKey();
            //    }); 

            
        }
    }
}
