using EmpolyeeManagment.Models;
using EmpolyeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Controllers
{
    [Authorize(Roles = "Manager,Admin", Policy ="CreateRole") ]

    public class AdminstrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPasswordHasher<ApplicationUser> passwordHasher;
        private readonly IDataProtector protector;

        public AdminstrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
                                        IPasswordHasher<ApplicationUser> passwordHasher, IDataProtectionProvider dataprovider)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
            protector = dataprovider.CreateProtector("hello"); // to decrypt and encrypt data
        }

        [HttpGet]
        [Authorize(Policy = "CreateRole")]
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreateRole")]
        public async Task<ActionResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole { Name = model.Name };
                IdentityResult saveRole = await roleManager.CreateAsync(role);
                if (saveRole.Succeeded)
                    return RedirectToAction("AllRoles");
                foreach (IdentityError error in saveRole.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult AllRoles()
        {
            var model = roleManager.Roles;

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "UpdateRole")]
        public async Task<ActionResult> EditRole(string id)
        {
            var find = await roleManager.FindByIdAsync(id);
            //  var usersRole =  userManager.Users ;
            var model = new EditRoleViewModel { RoleName = find.Name, Id = find.Id, Concurrency = find.ConcurrencyStamp };

            foreach (var user in userManager.Users)
                if (await userManager.IsInRoleAsync(user, find.Name))
                    model.Users.Add(user.UserName);

            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "UpdateRole")]
        public async Task<ActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //update the role 
                var role = new IdentityRole { Id = model.Id, Name = model.RoleName, ConcurrencyStamp = model.Concurrency };
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("AllRoles", "adminstration");
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "DeleteRole")]
        public async Task<ActionResult> Delete(string id)
        {
            var find = await roleManager.FindByIdAsync(id);
            if (find != null)
            {
                var model = new RoleViewModel { Id = find.Id, Name = find.Name, Concurrency = find.ConcurrencyStamp };
                return View(model);
            }

            return RedirectToAction("AllRoles");
        }

        [HttpPost]
        [Authorize(Policy ="DeleteRole")]
        public async Task<ActionResult> Delete(EditRoleViewModel model)
        {
            var delete = new IdentityRole { Id = model.Id, Name = model.RoleName, ConcurrencyStamp = model.Concurrency };
            var result = await roleManager.DeleteAsync(delete);
            if (result.Succeeded)
                return RedirectToAction("AllRoles");
            return View(model);
        }

        [AcceptVerbs("post", "get")]
        public async Task<ActionResult> CheckRoleNameExist(string Name)
        {
            var find = await roleManager.FindByNameAsync(Name);
            if (find != null)
                return Json("this Role Name is already exist");
            return Json(true);
        }

        [HttpGet]
        [Authorize(Policy = "CreateRole")]
        public async Task<ActionResult> AddRoleUsers(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            var model = new AddRoleUserViewModel { RoleId = roleId };
            foreach (var user in userManager.Users)
                if (!await userManager.IsInRoleAsync(user, role.Name))
                    model.Users.Add(user);

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "CreateRole")]
        public async Task<ActionResult> AddRoleUsers(AddRoleUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //add user to role 
                var user = await userManager.FindByIdAsync(model.UserId);
                var roleName = await roleManager.FindByIdAsync(model.RoleId);
                var add = await userManager.AddToRoleAsync(user, roleName.NormalizedName);

                if (add.Succeeded)
                    return RedirectToAction("AddRoleUsers", new { roleId = model.RoleId });

                foreach (var error in add.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "DeleteRole")]
        public async Task<ActionResult> RemoveRoleUsers(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            var model = new AddRoleUserViewModel { RoleId = roleId };
            foreach (var user in userManager.Users)
                if (await userManager.IsInRoleAsync(user, role.Name))
                    model.Users.Add(user);

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRole")]
        public async Task<ActionResult> RemoveRoleUsers(AddRoleUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //remove user to role 
                var user = await userManager.FindByIdAsync(model.UserId);
                var roleName = await roleManager.FindByIdAsync(model.RoleId);
                var add = await userManager.RemoveFromRoleAsync(user, roleName.NormalizedName);

                if (add.Succeeded)
                    return RedirectToAction("RemoveRoleUsers", new { roleId = model.RoleId });

                foreach (var error in add.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> AllUsers()
        {
            var model = userManager.Users;
            var CheckManager = await userManager.FindByNameAsync(User.Identity.Name); 
            if(await userManager.IsInRoleAsync(CheckManager, "Manager")) {
                return View(model);
            }
            var NewModel = new List<ApplicationUser>();
            foreach (var user in model)
            {
                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    NewModel.Add(user); 
                }
                if (await userManager.IsInRoleAsync(user, "User"))
                {
                    NewModel.Add(user); 
                }
            }
            return View(NewModel);
        }

        [HttpGet]
        [Authorize(Policy = "CreateRole")]
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "CreateRole")]
        public async Task<ActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if the model is validated then add user 
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, City = model.City, Department = model.Department };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("AllUsers", "adminstration");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View();
            }


            ModelState.AddModelError("", "Please add a valid data");
            return View();
        }
        [HttpGet]
        [Authorize(Policy = "UpdateRole")]
        public async Task<ActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                City = user.City,
                Department = user.Department
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "UpdateRole")]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //check if user change email with exist or not 
                var CheckEmail = await userManager.FindByEmailAsync(model.Email);
                if (CheckEmail != null)
                {
                    if (CheckEmail.Id != model.Id)
                    {
                        ModelState.AddModelError("Email", "The Email is Already Exist");
                        return View(model);
                    }

                }
                var findUser = await userManager.FindByIdAsync(model.Id);
                findUser.UserName = model.Name;
                findUser.Email = model.Email;
                findUser.City = model.City;
                findUser.Department = model.Department;
                if (model.Password != null)
                {
                    //here to hash the password and pass it
                    findUser.PasswordHash = passwordHasher.HashPassword(findUser, model.Password);
                }
                //update 
                var result = await userManager.UpdateAsync(findUser);
                if (result.Succeeded)
                    return RedirectToAction("EditUser", new { model.Id });
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);

        }

        [HttpGet]
        [Authorize(Policy = "DeleteRole")]
        public async Task<ActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var model = new DeleteUserViewModel { Id = user.Id, Email = user.Email, Name = user.UserName };
                return View(model);
            }
            return RedirectToAction("AllUsers");
        }
        [HttpPost]
        [Authorize(Policy = "DeleteRole")]
        public async Task<ActionResult> DeleteUser(DeleteUserViewModel model)
        {
            var user = userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user.Result);
                if (result.Succeeded)
                    return RedirectToAction("AllUsers");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> AllUserRoles(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                ViewBag.Name = user.UserName;
                var roles = roleManager.Roles;
                var model = new UserRoleViewModel { UserId = user.Id };
                foreach (var role in roles)
                {
                    var userRole = new UserRole { RoleName = role.Name };
                    if (await userManager.IsInRoleAsync(user, role.Name))
                        userRole.IsSelected = true;
                    model.UserRoles.Add(userRole);
                }
                return View(model);

            }
            return View(new UserRoleViewModel { UserId = null });
        }
        [HttpPost]
        public async Task<ActionResult> AllUserRoles(UserRoleViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            ViewBag.Name = user.UserName;
            if (user != null)
            {
                //delete all roles for this user and add the selected 
                var ExistingRoles = await userManager.GetRolesAsync(user);
                var delete = await userManager.RemoveFromRolesAsync(user, ExistingRoles);
                if (delete.Succeeded)
                {
                    // add roles  
                    foreach (var role in model.UserRoles)
                    {
                        if (role.IsSelected == true)
                        {
                            await userManager.AddToRoleAsync(user, role.RoleName);
                        }
                    }

                }

            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "MakeChanges")]
        public async Task<ActionResult> AllUserClaims(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                //create claim model 
                var existingUserClaim = await userManager.GetClaimsAsync(user);
                var model = new UserClaimViewModel { UserId = id };
                foreach (var claim in ClaimsStore.AllClaims)
                {
                    var userClaim = new UserClaim
                    {
                        ClaimType = claim.Type,
                    };

                    if (existingUserClaim.Any(a => a.Type == claim.Type))
                        userClaim.IsSelected = true;
                    else
                        userClaim.IsSelected = false;

                    model.Claims.Add(userClaim);
                }

                ViewBag.Name = user.UserName;
                return View(model);
            }
            return RedirectToAction("AllUsers");
        }

        [HttpPost]
        [Authorize(Policy = "MakeChanges")]
        public async Task<ActionResult> AllUserClaims(UserClaimViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            ViewBag.Name = user.UserName;
            if (user != null)
            {
                //delete all the claims 
                var delete = await userManager.RemoveClaimsAsync(user, ClaimsStore.AllClaims);
                if (delete.Succeeded)
                {
                    var add = new List<Claim>();
                    foreach (var claim in model.Claims)
                    {
                        if (claim.IsSelected == true)
                            add.Add(ClaimsStore.AllClaims.Where(f => f.Type == claim.ClaimType).FirstOrDefault());
                    }
                    var update = await userManager.AddClaimsAsync(user, add);

                    foreach (IdentityError error in update.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        [AllowAnonymous]
        [Route("Account/AccessDenied")]
        public ActionResult RedirectIfNotAuthorize(string ReturnUrl)
        {
            if (LocalRedirect(ReturnUrl) != null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = ReturnUrl });
            }
            return RedirectToAction("index", "home");
        }
        public JsonResult CheckEmailDomain(string email)
        {
            var values = email.Split('@');
            if (values[1] == "medo.com")
                return Json(true);
            return Json(false);
        } 

        public JsonResult Test()
        {
            List<string> values = new(); 
            values.Add( protector.Protect("lol") );
            values.Add( protector.Unprotect(values[0]) ) ;

            return Json(values);
        }
    }
}
