using EmpolyeeManagment.Models;
using EmpolyeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Controllers
{ 
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IPasswordHasher<ApplicationUser> passwordHasher;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if the model is validated then add user 
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, City = model.City, Department = model.Department };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("ConfirmEmail", new { userId = user.Id, token = await userManager.GenerateEmailConfirmationTokenAsync(user) }); 
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View();
            }


            ModelState.AddModelError("", "Please add a valid data");
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public async Task<ActionResult> Login(string ?returnUrl)
        {
            var model = new LoginViewModel {
                ReturnUrl = returnUrl, 
                ExternalProvider = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (model.ExternalProvider == null)
            {
                model.ExternalProvider = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            };

            if (ModelState.IsValid)
            {
                // check if email is confirmed or not 
                var user = await userManager.FindByNameAsync(model.UserName); 
               if( user != null && !user.EmailConfirmed &&( await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError("", "Please Confirm your Email first");
                    ViewBag.Confirm = true;
                    ViewBag.userId = user.Id;
                    ViewBag.token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    return View(model); 
                }
                
                //login 
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (model.ReturnUrl != null)
                        return Redirect(model.ReturnUrl);
                    else
                        return RedirectToAction("index", "home"); 
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                ModelState.AddModelError("", "Invalid login attempt."); 
            } 
        
            return View(model);

        } 

        public async Task<ActionResult> IsUserNameExist (string Name)
        {
            var validate = await userManager.FindByNameAsync(Name);
            if (validate != null)
                return Json($"the {Name} is already used");
            return Json(true); 
        } 

        public async Task<ActionResult> IsEmailExist(string Email)
        {
            var validate = await userManager.FindByEmailAsync(Email);
            if (validate != null)
                return Json($"the {Email} is Already Exist");
            return Json(true); 
        } 

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalProvider(string provider, string returnUrl)
        {
            var url = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var options =  signInManager.ConfigureExternalAuthenticationProperties(provider, url); 
          
           return new ChallengeResult(provider, options);

        }
        [AllowAnonymous]
        public async Task<IActionResult>ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProvider =  (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);

            if (user != null && !user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please Confirm your Email first");
                ViewBag.Confirm = true;
                ViewBag.userId = user.Id;
                ViewBag.token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                return View("Login", loginViewModel);
            }


            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                if (email != null)
                {
                    // Create a new user without password if we do not have a user already

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    ModelState.AddModelError("", "Please Confirm your Email first");
                    ViewBag.Confirm = true;
                    ViewBag.userId = user.Id;
                    ViewBag.token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    return View("Login", loginViewModel);
                }

                // If we cannot find the user email we cannot continue
                ModelState.AddModelError("", $"Email claim not received from: {info.LoginProvider}");
                ModelState.AddModelError("",  "Please contact support on mohamedaahmed92@gmail.com");

                return View("Login", new {  returnUrl });
            }

        }
        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId); 
            if(user != null)
            {
                var confirm = await userManager.ConfirmEmailAsync(user, token);
                if (confirm.Succeeded)
                {
                    return View(); 
                }
                ViewBag.ErrorMessage = "You should try to login first";
                return View("../CustomError");

            }

            ViewBag.ErrorMessage = "User Is not found please login again to confirm";
            return View("../CustomError");
        } 

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View(); 
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task< ActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email); 
                if(user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var newModel = new RestPasswordViewModel { Token = token };
                    return View("RestPassword", newModel); // should here send the token to the email 
                }
                ModelState.AddModelError("", "you type invalid email"); 
            }

            return View(model); 
        } 

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RestPassword(RestPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && user.EmailConfirmed)
                {
                    // save the new Password  
                    var rest = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (rest.Succeeded)
                    { 
                        if(await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user,DateTime.UtcNow); 
                        }
                        return RedirectToAction("login");
                    }
                    foreach (IdentityError error in rest.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model); 
                }
                ViewBag.ErrorMessage = "You should try to login first";
                return View("../CustomError");
            }

            return View(model); 
        } 

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> AddPassword()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user.PasswordHash != null) {
                return RedirectToAction("ChangePassword"); 
            }
            var model = new AddPassword { Id = user.Id};
            return View(model); 
        } 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddPassword(AddPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id); 
                if(user != null)
                {
                    if(model.NewPassword == model.ConfirmPassword)
                    {
                        var change = await userManager.AddPasswordAsync(user, model.NewPassword);
                        await signInManager.RefreshSignInAsync(user); 
                        if (change.Succeeded)
                            return RedirectToAction("index", "home");
                        foreach (IdentityError error in change.Errors)
                            ModelState.AddModelError("", error.Description);
                        return View(model);
                    }
                }
                ModelState.AddModelError("","User Is not found right now in the database"); 
            }
            return View(model);
        } 

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ChangePassword()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var model = new ChangePasswordViewModel { Id = user.Id }; 
            return View(model); 
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id); 
                if(user != null)
                {
                    if(model.NewPassword == model.ConfirmPassword)
                    {
                        var change = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                        if (change.Succeeded)
                        {
                            await signInManager.RefreshSignInAsync(user); 
                            return RedirectToAction("index", "home"); 
                        }

                        foreach (var error in change.Errors)
                            ModelState.AddModelError("", error.Description);
                        return View(model); 

                    }
                    ModelState.AddModelError("", "the two password didn't match each other");
                    return View(model);
                }
                ModelState.AddModelError("", "user is not found right now"); 
            }
            return View(model);
        }

    }
}
