using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ElmahCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
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
    }

}


namespace rkbc.web.controllers
{
    [Authorize(Roles = "Admin")]
    public class Administration : AppBaseController
    {
        private RoleManager<ApplicationRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signinManager;
        private readonly ILogger _logger;

        public Administration(RoleManager<ApplicationRole> roleMag,
                                        UserManager<ApplicationUser> userMag,
                                        SignInManager<ApplicationUser> signinMag,
                                        ILogger<Administration> logger,
                                        IUnitOfWork _unitOfWork,
                                        UserService _userService
                                        ) :base(_unitOfWork, _userService)
        {
            roleManager = roleMag;
            userManager = userMag;
            signinManager = signinMag;
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
           
            userService.logOffUser();
            return RedirectToAction("Index", "Home");
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
            return vm;
        }

        [AllowAnonymous]
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
                phoneNumber = q.PhoneNumber,
                address1 = q.address1,
                roles = q.UserRoles != null ? q.UserRoles.Select(r => r.Role.Name).ToList() : null
            }).ToListAsync();

            return View(users);
        }
        
        public async Task<IActionResult> Create()
        {
            //permission
            ApplicationUser user = new ApplicationUser();
            user.officeId = 1;
            user.countryCode = 1;
            
            //var identityUser = await user.GenerateUserIdentityAsync(userManager);
            
            var vm = await setupViewModel(user, FormViewMode.Create);
            return View("Edit",vm);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            ApplicationUser modelObj = new ApplicationUser();
            var vModel = new AppUserViewModel();
            await TryUpdateModelAsync<AppUserViewModel>(vModel);
            string defaultPassWord = "1234567";
            modelObj.officeId = 1;
            modelObj.countryCode = 1;
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
                    string userErrors = "";
                    foreach (var err in userResult.Errors)
                        userErrors = String.Join(",", userResult.Errors);
                    ModelState.AddModelError("user", userErrors);
                }

            }
            var vm = setupViewModel(modelObj, FormViewMode.Create);

            return View("Edit", vm);
        }
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Details(string id, FormViewMode mode = FormViewMode.View)
        {
            var query = addModelIncludes(userManager.Users.OrderBy(q => q.lastName).Where(q => q.Id == id));
            var user = await query.FirstAsync();
            var vm = await setupViewModel(user, mode);
            return View("Details", vm);
        }
        
        public async Task<IActionResult> Edit(string id)
        {
            var user = await addModelIncludes(userManager.Users.OrderBy(q => q.lastName).Where(q => q.Id == id)).FirstOrDefaultAsync();
            
            var vm = await setupViewModel(user, FormViewMode.Edit);
            return View("Edit", vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit()
        {
            var vModel = new AppUserViewModel();
            await TryUpdateModelAsync<AppUserViewModel>(vModel);
            ApplicationUser modelObj = await addModelIncludes(userManager.Users.OrderBy(q => q.lastName).Where(q => q.Id == vModel.id)).FirstOrDefaultAsync();
            if (modelObj.createdDate == null)
                modelObj.createdDate = DateTime.UtcNow;

            acceptPost(modelObj, vModel, FormViewMode.Edit);
            if (ModelState.IsValid)
            {
                var userResult = await userManager.UpdateAsync(modelObj);
                if (userResult.Succeeded)
                {
                    if(modelObj.UserRoles.Count > 0)
                    {
                        var roles = modelObj.UserRoles.Select(q => q.Role.Name).ToArray();
                        var result = await userManager.RemoveFromRolesAsync(modelObj, roles);
                    }
                    
                    var roleResult = await userManager.AddToRolesAsync(modelObj, vModel.roles);
                    if (roleResult.Succeeded)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError("roles", "failed");
                }
                else
                {
                    string userErrors = "";
                    foreach (var err in userResult.Errors)
                        userErrors = String.Join(",", userResult.Errors);
                    ModelState.AddModelError("user", userErrors);
                }

            }
            var vm = await setupViewModel(modelObj, FormViewMode.Edit);

            return View("Edit", vm);
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            var user = await addModelIncludes(userManager.Users.OrderBy(q => q.lastName).Where(q => q.Id == id)).FirstOrDefaultAsync();
            if (user == null) throw new InvalidOperationException("Attempted to delete an user who does not exist.");
            if (user == null) HttpContext.RiseError(new InvalidOperationException("Test"));

            var currentUserEmail = User.Identity.Name;
            
            if (currentUserEmail == user.Email) 
            {
                HttpContext.RiseError(new InvalidOperationException("The current user cann't be deleted"));
                userManager.Logger.LogError("The current user cann't be deleted");
                TempData["errors"] = "The current user cann't be deleted";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Details", new { id = user.Id, mode = FormViewMode.Delete });
            
        }
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await addModelIncludes(userManager.Users.OrderBy(q => q.lastName).Where(q => q.Id == id)).FirstOrDefaultAsync();
            if (user == null) throw new InvalidOperationException("Attempted to delete an user who does not exist.");
            if (user == null) userManager.Logger.LogError("Null value");
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                userManager.Logger.LogError("", "Bad Request.");
                throw new InvalidOperationException("Unable to delete user " + user.firstName + " " + user.lastName);
            }

            return RedirectToAction("index");
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
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