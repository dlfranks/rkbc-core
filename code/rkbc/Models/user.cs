using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rkbc.core.models
{
    
    public class CustomClaimTypes
    {
        public const string Permission = "projectname/permission";
    }
    public static class UserPermission
    {
        public const string View = "users.view";
        public const string Create = "users.create";
        public const string Update = "users.edit";
        public const string Delete = "users.delete";
    }

    public static class Teams
    {
        public const string AddRemove = "teams.addremove";
        public const string EditManagers = "teams.edit.managers";
        public const string Delete = "teams.delete";
    }
    public enum RoleEnum
    {
        [EnumMember(Value = "Admin")]
        Admin,
        [EnumMember(Value = "User")]
        User,
    }
    public enum GenderEnum
    {
        [Display(Name = "Male")]
        Male = 1,
        [Display(Name = "Female")]
        Female = 2
    }
    public enum GroupEnum
    {
        [Display(Name = "Women")]
        Women = 1,
        [Display(Name = "Man")]
        Man = 2,
        [Display(Name = "Young Adult")]
        YoungAgult = 3,
        [Display(Name = "Youth")]
        Youth = 4,
        [Display(Name = "Elementary")]
        Elementary = 5,
        [Display(Name = "Toddler")]
        Toddler = 6
    }
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser()
        {
            
        }

        public int? officeId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int? department { get; set; }
        public int? gender { get; set; }
        public DateTime? DOB { get; set; }
        public string address1 { get; set; }
        public int? countryCode { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? updatedDate { get; set; }

        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public async Task<ClaimsIdentity> GenerateUserClaimsIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userClaimsIdentity = new ClaimsIdentity(await manager.GetClaimsAsync(this), CookieAuthenticationDefaults.AuthenticationScheme);
            var roles = await manager.GetRolesAsync(this);
            userClaimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, this.Email));
            userClaimsIdentity.AddClaim(new Claim(ClaimTypes.Name, this.UserName));

            foreach (var role in roles)
            {
                userClaimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }


            return userClaimsIdentity;
        }

    }
 
    public class ApplicationRole : IdentityRole<string>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }
    
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }

    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }

    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }


    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public UserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
                : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("department", GroupEnum.Man.ToString()));
            return identity;
        }
    }
    public class UserActivityLog : IEntity
    {
        public int id { get; set; }

        public DateTime activityDate { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string activity { get; set; }
        public bool isMobileInterface { get; set; }
        public string userAgent { get; set; }
        public string offices { get; set; }
    }
}
