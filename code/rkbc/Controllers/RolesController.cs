using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.viewmodels;

namespace rkbc.web.viewmodels
{
    public class RoleViewModel
    {
        public string roleId { get; set; }
        public string roleName { get; set; }
    }
}

namespace rkbc.web.controllers
{
    
    public class RolesController : AppBaseController
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        
        public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userMag,
                                IUnitOfWork _unitOfWork, UserService _userService) : base(_unitOfWork, _userService)
        {
            this.roleManager = roleManager;
            userManager = userMag;
            
        }
        public IActionResult Index()
        {
            List<RoleViewModel> vm = new List<RoleViewModel>();
            vm = roleManager.Roles.Include("UserRoles").Select(q => new RoleViewModel
            {
                roleId = q.Id,
                roleName = q.Name
            }).ToList();
            return View("Index", vm);
        }

        public IActionResult Create()
        {
           
            ViewBag.formViewMode = FormViewMode.Create;
            return View("Role", new RoleViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole modelObj = new ApplicationRole
                {
                    Name = model.roleName
                };
                IdentityResult result = await roleManager.CreateAsync(modelObj);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.formViewMode = FormViewMode.Create;
            return View("Role", model);
        }

        public async Task<IActionResult> Edit(string name)
        {
            var role = await roleManager.FindByNameAsync(name);
            ViewBag.formViewMode = FormViewMode.Edit ;
            return View("Role", new RoleViewModel() { roleId = role.Id, roleName = role.Name});
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            var modelObj = await roleManager.FindByNameAsync(model.roleName);
            modelObj.Name = model.roleName;
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.UpdateAsync(modelObj);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.formViewMode = FormViewMode.Edit;
            return View("Role", model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var role =  roleManager.Roles.Include("UserRoles").Where(q => q.Id == id).FirstOrDefault();
            var model = new RoleViewModel() { roleId = id, roleName = role.Name };
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Role", model);
            }
            else
            {
                
                if (role.UserRoles.Count > 0)
                {
                    return View("Role", model);
                }
                
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("Role", model); 
                }
            }
            
            
        }
    }
}