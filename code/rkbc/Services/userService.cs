﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rkbc.web.extensions;
using Microsoft.AspNetCore.Identity;
using rkbc.core.models;
using Microsoft.Extensions.Logging;
using RKBC.Areas.Identity.Pages.Account;

namespace rkbc.core.service
{
    public class UserSettings
    {
        public IList<string> roles { get; set; }

        public string userName { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string userId { get; set; }
        public bool _isAdmin { get; set; }
        public bool isAdmin { 
            get 
            {
                return (isAuthenticated && _isAdmin);
            } 
        }
        public bool isAuthenticated
        {
            get
            {
                return (!String.IsNullOrEmpty(userId));
            }
        }
        public bool isInRole(string role)
        {
            if (!isAuthenticated)
                return (false);
            if (roles == null)
                return (false);
            if (String.IsNullOrWhiteSpace(role))
                return (false);
            return roles.Contains(role);
        }
    }
    public class UserService
    {
        IHttpContextAccessor httpContextAccessor;
        HttpContext httpContext;
        UserManager<ApplicationUser> userManager;
        SignInManager<ApplicationUser> signinManger;
        private readonly ILogger<UserService> logger;
        const string SessionName = "__UserSettings";
        public UserService(IHttpContextAccessor _httpContextAccessor, 
                            UserManager<ApplicationUser> _userManager,
                            SignInManager<ApplicationUser> _signinManger,
                            ILogger<UserService> _logger)
        {
            this.httpContextAccessor = _httpContextAccessor;
            httpContext = _httpContextAccessor.HttpContext;
            userManager = _userManager;
            signinManger = _signinManger;
            logger = _logger;
        }
        public UserSettings CurrentUserSettings{
            get {
                UserSettings us = httpContext.Session.GetObject<UserSettings>(SessionName);
                
                logger.LogInformation("us: start, UserSettings in UserService");
                if (us == null)
                {
                    logger.LogInformation("us: null, UserSettings in UserService");
                    us = new UserSettings();
                    if(httpContext.User.Identity.IsAuthenticated)
                    {
                        logger.LogInformation("httpContext.User.Identity.IsAuthenticated: true, UserSettings in UserService");
                        var task = Task.Run(async () => {
                            if (httpContext.User.Identity.Name != null)
                                return await userManager.FindByNameAsync(httpContext.User.Identity.Name);
                            else
                               throw new InvalidOperationException("User.Identity goes wrong.");
                            
                        });
                        
                        var user = task.Result;
                        if (task.Result != null)
                        {
                            logger.LogInformation("retrieved user: true, UserSettings in UserService");
                            us.email = user.Email;
                            us.fullName = user.firstName + " " + user.lastName;
                            us.userId = user.Id;
                            us.roles = Task.Run(async () => { return await userManager.GetRolesAsync(user); }).Result;
                            us._isAdmin = false;
                            if (us.roles != null && us.roles.Count > 0)
                                us._isAdmin = us.roles.Any(q => q == RoleEnum.Admin.ToString());
                            //set Sesstion
                            httpContext.Session.SetObject(SessionName, us);
                        }
                        else
                        {
                            logger.LogInformation("retrieved user: false, UserSettings in UserService");
                            Task.Run(async () => await logOffUser());
                        }
                    }
                    else {
                        logger.LogInformation("httpContext.User.Identity.IsAuthenticated: false, UserSettings in UserService");
                        Task.Run(async () => await logOffUser()); 
                    }

                }
                logger.LogInformation("us: has values, UserSettings in UserService");
                return us;
            }
        }
        public async Task logOffUser()
        {
            if(httpContext.Request != null && httpContext.Response != null)
            {
                await signinManger.SignOutAsync();
                
                
                httpContext.Session.Clear();
            }
        }

        
    }
}
