using EmpolyeeManagment.Models;
using EmpolyeeManagment.Models.Interfaces;
using EmpolyeeManagment.Models.Repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace EmpolyeeManagment
{
    public class Startup
    {
        private readonly IConfiguration config;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940 
        public Startup(IConfiguration _config)
        {
            config = _config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //add database connection
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(config.GetConnectionString("SqlServerConnection")) 
                                  .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            //add identity 
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); 
          
            services.AddMvc();

            services.AddAuthentication()
                .AddGoogle(options => {
                options.ClientId = "168405875295-qmpqhoqnbk9bnjvr082rpeorn05i4u71.apps.googleusercontent.com"; 
                options.ClientSecret = "kw1znhw6T-cFc7rBXgprq5Vy";
                //options.SignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddFacebook(options => {
                options.AppId = "359045449169731";
                options.AppSecret = "e9fbae43e5b880aa1f9116473a098ebd";
            });
            services.AddScoped<IEmployeeCollection<Employee>, EmployeeDbRepo>();

            services.Configure<IdentityOptions>(options => {
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
            });

           // services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(10));  // override the default token time 

            //add policy for claims 
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CreateRole", policy => { policy.RequireClaim("Create User"); });
                options.AddPolicy("UpdateRole", policy => { policy.RequireClaim("Update User"); });
                options.AddPolicy("DeleteRole", policy => { policy.RequireClaim("Delete User"); });
                options.AddPolicy("MakeChanges", policy => { policy.RequireClaim("Make Changes"); });
                //options.AddPolicy("SuperAdmin", policy => {
                //    policy.RequireAssertion(select => select.User.IsInRole("Manager") ||
                //        select.User.HasClaim("Delete User", "Delete User") && select.User.IsInRole("Admin"));
                //});
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseStatusCodePagesWithReExecute("/Error/{0}");


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //add identity middleware 
            app.UseAuthentication();
            app.UseAuthorization(); 

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });


            //app.Run(async (context) =>
            //   {
            //      await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            //    }); 


        }
    }
}
