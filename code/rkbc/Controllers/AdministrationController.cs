using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ElmahCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class AppUserViewModel
    {
        public AppUserViewModel()
        {
            roles = new List<string>();
        }
        
        public string id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Gender")]
        [EnumDataType(typeof(GenderEnum))]
        public int? gender { get; set; }

        [Display(Name = "Group")]
        [EnumDataType(typeof(GroupEnum))]
        public int? department { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyy}")]
        public DateTime? DOB { get; set; }

        [Display(Name = "Phone No")]
        [DataType(DataType.PhoneNumber)]
        public string phoneNumber { get; set; }

        [Display(Name = "Address")]
        public string address1 { get; set; }

        [NotMapped]
        [Display(Name = "Roles")]
        public IList<string> roles { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int countryCode { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public int accountType { get; set; }
    }

}


namespace rkbc.web.controllers
{
    [Authorize(Roles="Admin, User")]
    public class AdministrationController : AppBaseController
    {
        private RoleManager<ApplicationRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signinManager;
        private readonly ILogger _logger;
        protected readonly IOptions<RkbcConfig> rkbcConfig;

        public AdministrationController(RoleManager<ApplicationRole> roleMag,
                                        UserManager<ApplicationUser> userMag,
                                        SignInManager<ApplicationUser> signinMag,
                                        ILogger<AdministrationController> logger,
                                        IOptions<RkbcConfig> _rkbcConfig,
                                        IUnitOfWork _unitOfWork,
                                        UserService _userService
                                        ) : base(_unitOfWork, _userService)
        {
            roleManager = roleMag;
            userManager = userMag;
            signinManager = signinMag;
            _logger = logger;
            rkbcConfig = _rkbcConfig;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            
            this.ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/Home/Index");
            LoginViewModel vm = new LoginViewModel();
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ExternalLogins = (await signinManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl = returnUrl ?? Url.Content("~/Home/Index");

            if (ModelState.IsValid)
            {
                var user = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").Where(q => q.Email == model.Email).FirstOrDefaultAsync();
                if (user != null)
                {
                    if(await userService.tryLogOnUser(user, model.RememberMe))
                        return Redirect("~/Home/Index");
                }else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            
            await userService.logOffUser();
            
            return Redirect("~/Home/Index");

        }
        protected IQueryable<ApplicationUser> addModelIncludes(IQueryable<ApplicationUser> query)
        {
            return (query.Include("UserRoles").Include("UserRoles.Role").Include("Claims"));
        }
        protected void acceptPost(ApplicationUser modelObj, AppUserViewModel vModel, FormViewMode mode)
        {
            Dictionary<string, string> errors;
            
            modelObj.firstName = vModel.firstName;
            modelObj.lastName = vModel.lastName;
            modelObj.Email = vModel.Email;
            modelObj.gender = vModel.gender;
            modelObj.department = vModel.department;
            modelObj.PhoneNumber = vModel.phoneNumber;
            modelObj.address1 = vModel.address1;
            modelObj.updatedDate = DateTime.UtcNow;
            modelObj.DOB = vModel.DOB;
            modelObj.accountType = vModel.accountType;
            modelObj.countryCode = vModel.countryCode;
        }
        protected async Task<AppUserViewModel> setupViewModel(ApplicationUser appUser, FormViewMode mode)
        {
            AppUserViewModel vm = new AppUserViewModel();
            vm.id = appUser.Id;
            vm.firstName = appUser.firstName;
            vm.lastName = appUser.lastName;
            vm.Email = appUser.Email;
            vm.DOB = appUser.DOB;
            vm.department = appUser.department;
            vm.gender = appUser.gender;
            vm.phoneNumber = appUser.PhoneNumber;
            vm.address1 = appUser.address1;
            vm.countryCode = appUser.countryCode?? (int)CountryEnum.USA;
            vm.accountType = appUser.accountType?? (int)AccountType.Personal;

            
            if (mode != FormViewMode.Create && appUser.UserRoles.Count > 0)
            {
                vm.roles = appUser.UserRoles.Select(q => q.Role.Name).ToList();
            }
            else
            {
                vm.roles.Add(RoleEnum.User.ToString());
            }
            var allRoles = await roleManager.Roles.Select(q => new RoleViewModel()
            {
                roleId = q.Id,
                roleName = q.Name
            }).ToListAsync();
            ViewBag.roleList = allRoles;
            ViewBag.formViewMode = mode;
            ViewBag.UserSettings = userService.CurrentUserSettings;
            ViewBag.CountryList = rkbc.web.constant.Constants.getCountryList();
            return vm;
        }

        public async Task<IActionResult> Index()
        {
            List<AppUserViewModel> users = new List<AppUserViewModel>();
            var allUsers = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").ToListAsync();
            var query = addModelIncludes(userManager.Users.OrderBy(q => q.lastName));
            users = await query.Select(q => new AppUserViewModel
            {
                id = q.Id,
                firstName = q.firstName,
                lastName = q.lastName,
                Email = q.Email,
                department = q.department,
                gender = q.gender,
                DOB = q.DOB,
                countryCode = q.countryCode?? (int)CountryEnum.USA,
                accountType = q.accountType?? (int)AccountType.Personal,
                roles = q.UserRoles != null ? q.UserRoles.Select(r => r.Role.Name).ToList() : null
            }).ToListAsync();

            return View(users);
        }
        
        public async Task<IActionResult> Create()
        {
            //permission
            
            ApplicationUser user = new ApplicationUser();
            user.officeId = 1;
            user.countryCode = (int)CountryEnum.USA;
            user.accountType = (int)AccountType.Personal;
            var vm = await setupViewModel(user, FormViewMode.Create);
            return View("Edit",vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            ApplicationUser modelObj = new ApplicationUser();
            var vModel = new AppUserViewModel();
            await TryUpdateModelAsync<AppUserViewModel>(vModel);
            var defaultPassWord = rkbcConfig.Value.DefaultPassword;

            modelObj.officeId = 1;
            modelObj.UserName = vModel.Email;
            modelObj.createdDate = DateTime.UtcNow;
            acceptPost(modelObj, vModel, FormViewMode.Create);
            if (ModelState.IsValid)
            {
                var userResult = await userManager.CreateAsync(modelObj, defaultPassWord);
                if (userResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRolesAsync(modelObj, vModel.roles);
                    if (roleResult.Succeeded)
                    {
                        var isAdmin = await userManager.IsInRoleAsync(modelObj, RoleEnum.Admin.ToString());
                        if (isAdmin)
                        {
                            await userManager.AddClaimAsync(modelObj, new Claim(CustomClaimTypes.Permission, UserPermission.Create));
                            await userManager.AddClaimAsync(modelObj, new Claim(CustomClaimTypes.Permission, UserPermission.Update));
                            await userManager.AddClaimAsync(modelObj, new Claim(CustomClaimTypes.Permission, UserPermission.Delete));
                        }
                        await userManager.AddClaimAsync(modelObj, new Claim(CustomClaimTypes.Permission, UserPermission.View));
                        return RedirectToAction("Details", new { id = modelObj.Id });
                    }
                    else
                        ModelState.AddModelError("roles", "failed");
                }
                else
                {
                    string userErrors = "Failed to save.";
                    
                    ModelState.AddModelError("", userErrors);
                }

            }
            var vm = setupViewModel(modelObj, FormViewMode.Create);

            return View("Edit", vm);
        }
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Details(string id, FormViewMode mode = FormViewMode.View)
        {
            if(!userService.permissionForUserEditing(PermissionAction.Read, id, false)) return RedirectToAction("AccessDenied");
            var user = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").Where(q => q.Id == id).FirstOrDefaultAsync();
            var vm = await setupViewModel(user, mode);
            return View("Details", vm);
        }
        
        public async Task<IActionResult> Edit(string id)
        {
            if (!userService.permissionForUserEditing(PermissionAction.Update, id, false)) return RedirectToAction("AccessDenied");
           var user = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").Where(q => q.Id == id).FirstOrDefaultAsync();
            var vm = await setupViewModel(user, FormViewMode.Edit);
            return View("Edit", vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit()
        {
            var vModel = new AppUserViewModel();
            await TryUpdateModelAsync<AppUserViewModel>(vModel);
            ApplicationUser modelObj = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").Where(q => q.Id == vModel.id).FirstOrDefaultAsync();
            if (!userService.permissionForUserEditing(PermissionAction.Update, vModel.id, false)) return RedirectToAction("AccessDenied");
            if (modelObj.createdDate == null)
                modelObj.createdDate = DateTime.UtcNow;

            acceptPost(modelObj, vModel, FormViewMode.Edit);
            //Handle user's roles
            if (modelObj != null && modelObj.UserRoles.Count > 0 && vModel.roles.Count > 0)
            {

                var result = await userManager.RemoveFromRolesAsync(modelObj, modelObj.UserRoles.Select(q => q.Role.Name).ToArray());
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot removed the exsiting roles.");
                }
                else
                {
                    var roleResult = await userManager.AddToRolesAsync(modelObj, vModel.roles);
                   
                        
                }
            }
            //If user don't have any roles, default user's role
            if (modelObj.UserRoles.Count == 0)
            {
                var userRole = await roleManager.FindByNameAsync("User");
                if (userRole != null)
                {
                    modelObj.UserRoles.Add(new ApplicationUserRole() { User = modelObj, Role = userRole });
                }

            }
            if (ModelState.IsValid)
            {
                
                //update a user
                var userResult = await userManager.UpdateAsync(modelObj);
                if (userResult.Succeeded)
                {//check user's roles

                    return RedirectToAction("Index");


                }
                else
                {
                    string userErrors = "Failed to save.";
                    ModelState.AddModelError("", userErrors);
                }

            }
            var vm = await setupViewModel(modelObj, FormViewMode.Edit);

            return View("Edit", vm);
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            if (!userService.permissionForUserEditing(PermissionAction.Delete, id, false)) return RedirectToAction("AccessDenied");
            var user = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").Where(q => q.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Attempted to delete an user who does not exist.");
                
            }

            var currentUserEmail = User.Identity.Name;
            
            if (currentUserEmail == user.Email) 
            {
                HttpContext.RiseError(new InvalidOperationException("The current user cann't be deleted"));
                userManager.Logger.LogError("The current user cann't be deleted");
                TempData["message"] = "The current user cann't be deleted";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Details", new { id = user.Id, mode = FormViewMode.Delete });
            
        }
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!userService.permissionForUserEditing(PermissionAction.Update, userService.CurrentUserSettings.userId, false)) return RedirectToAction("AccessDenied");
            var user = await userManager.Users.Include("UserRoles").Include("UserRoles.Role").Where(q => q.Id == id).FirstOrDefaultAsync();
            if (user == null) throw new InvalidOperationException("Attempted to delete an user who does not exist.");
            //Does this user have posts to delete?
            var count = unitOfWork.posts.get().Where(q => q.blog.authorId == user.Id).Count();
            if(count > 0)
            {
                TempData["message"] = "All your posts must be deleted before deleting the user.";
                TempData["duration"] = "sticky";
                return RedirectToAction("Details", new { id = user.Id, mode = FormViewMode.Delete });
            }
            await userService.deleteUser(user);
            
            return RedirectToAction("index");
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<JsonResult> LookupEmail(string email)
        {
            var isValid = await userService.isValidEmail(email);
            return Json(new { isValid = isValid});
        }
        public async Task Setup()
        {
            var user = await userManager.FindByEmailAsync("cooldeana@gmail.com");

            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new ApplicationRole();
                adminRole.Name = "Admin";
                
                await roleManager.CreateAsync(adminRole);

                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermission.Create));
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermission.Update));
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermission.Delete));
                await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, UserPermission.View));
            }
            

            if (!await userManager.IsInRoleAsync(user, adminRole.Name))
            {
                await userManager.AddToRoleAsync(user, adminRole.Name);
            }

            var accountManagerRole = await roleManager.FindByNameAsync("Admin");

            
        }


    }
}