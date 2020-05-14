using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rkbc.web.extensions;
using Microsoft.AspNetCore.Identity;
using rkbc.core.models;
using Microsoft.Extensions.Logging;
using RKBC.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using rkbc.core.repository;

namespace rkbc.core.service
{
    
    public enum PermissionAction
    {
        Create,
        Read,
        Update,
        Delete,
        Index
    }
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
        IUnitOfWork unitOfWork;
        UserManager<ApplicationUser> userManager;
        SignInManager<ApplicationUser> signinManger;
        private readonly ILogger<UserService> logger;
        const string SessionName = "__UserSettings";
        public UserService(IHttpContextAccessor _httpContextAccessor,
                            IUnitOfWork _unitOfWork,
                            UserManager<ApplicationUser> _userManager,
                            SignInManager<ApplicationUser> _signinManger,
                            ILogger<UserService> _logger)
        {
            this.httpContextAccessor = _httpContextAccessor;
            httpContext = _httpContextAccessor.HttpContext;
            unitOfWork = _unitOfWork;
            userManager = _userManager;
            signinManger = _signinManger;
            logger = _logger;
        }
        public UserSettings CurrentUserSettings{
            get {
                UserSettings us = httpContextAccessor.HttpContext.Session.GetObject<UserSettings>(SessionName);
                
                logger.LogInformation("us: start, UserSettings in UserService");
                if (us == null)
                {
                    logger.LogInformation("us: null, UserSettings in UserService");
                    us = new UserSettings();
                    if(httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        logger.LogInformation("httpContext.User.Identity.IsAuthenticated: true, UserSettings in UserService");
                        var task = Task.Run(async () => {
                            if (httpContextAccessor.HttpContext.User.Identity.Name != null)
                                return await userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name);
                            else
                               throw new InvalidOperationException("User.Identity.Name hasn't been set.");
                            
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
                
                return us;
            }
        }
        public bool permissionForUserEditing(PermissionAction action, string id, bool autoThrow = false)
        {
            //Can't delete yourself no matter who you are
            if (CurrentUserSettings.userId == id && action == PermissionAction.Delete)
            {
                if (autoThrow) throw new InvalidOperationException("User " + CurrentUserSettings.userName + " does not have permission to " + action.ToString());
                return (false);
            }
            // Can do anything else to yourself
            if (CurrentUserSettings.userId == id) return (true);
            //Otherwise only these guys have permission to do anything else
            if (CurrentUserSettings.isAdmin) return (true);

            if (autoThrow) throw new InvalidOperationException("User " + CurrentUserSettings.userName + " does not have permission to " + action.ToString() );
            return (false);
        }
        public bool permissionForBlogEditing(PermissionAction action, string id, bool autoThrow = false)
        {
            // Can do anything else to yourself
            if (CurrentUserSettings.userId == id) return (true);
            //Otherwise only these guys have permission to do anything else
            if (CurrentUserSettings.isAdmin) return (true);

            if (autoThrow) throw new InvalidOperationException("User " + CurrentUserSettings.userName + " does not have permission to " + action.ToString());
            return (false);
        }
        public async Task logOffUser()
        {
            if(httpContext.Request != null && httpContext.Response != null)
            {

                await httpContextAccessor.HttpContext.SignOutAsync("AuthCookies");
                
                
                httpContext.Session.Clear();
            }
        }
        public async Task<bool> isValidEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user == null ? true : false;
        }
        public async Task<bool> tryLogOnUser(ApplicationUser user, bool rememberMe = false)
        {
            //var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Email.ToString(), ClaimValueTypes.String), new Claim(ClaimTypes.Name, user.UserName.ToString(), ClaimValueTypes.String)
            //    , new Claim(ClaimTypes.Role, "Admin", ClaimValueTypes.String), new Claim(ClaimTypes.Role, "User", ClaimValueTypes.String) };
            // var userClaimsIdentity = new ClaimsIdentity(claims, "AuthCookies");
            
            var userClaimsIdentity = await user.GenerateUserClaimsIdentityAsync(userManager);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(userClaimsIdentity);
            var authProperties = new AuthenticationProperties()
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(480),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = rememberMe,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            try
            {
                await httpContext.SignInAsync(
                "AuthCookies", claimsPrincipal, authProperties);
                logger.LogInformation("User logged in.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogInformation("Failed login.");
            }
            return false;
        }
        public async Task deleteUser(ApplicationUser user)
        {

            //delete Blog first if the user has a blog
            var blogModel = await unitOfWork.blogs.get().Where(q => q.authorId == user.Id).FirstOrDefaultAsync();
            
            if (blogModel != null)
            {
                unitOfWork.blogs.remove(blogModel);
                await unitOfWork.commitAsync();
            }
            
            //comment
            IList<Comment> comments = await unitOfWork.comments.get().Where(q => q.authorId == user.Id).ToListAsync();
            
            if (comments.Count > 0)
            {
                unitOfWork.comments.removeRange(comments);
                await unitOfWork.commitAsync();
            }
            var applicationuser = await userManager.FindByIdAsync(user.Id);
            if (applicationuser != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    userManager.Logger.LogError("User delete", "Can't delete the user.");
                    throw new InvalidOperationException("Unable to delete user " + user.firstName + " " + user.lastName);
                }
            }
                
        }
    }
}
