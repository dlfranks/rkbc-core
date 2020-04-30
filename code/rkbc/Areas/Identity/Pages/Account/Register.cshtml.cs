using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using rkbc.core.models;
using rkbc.core.service;

namespace RKBC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly UserService userService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<RegisterModel> logger,
            UserService _userService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            userService = _userService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Country")]
            public int countryCode { get; set; }

            [Required]
            [Display(Name = "Account Type")]
            public int accountType { get; set; }

        }
        public bool LookupEmail(string email)
        {
           var user = _userManager.FindByEmailAsync(email);
            return user == null ? true : false;
        }
        public void OnGet(string returnUrl = null)
        {
           ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByNameAsync("user");
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email};
                user.officeId = 1;
                user.countryCode = Input.countryCode;
                user.accountType = Input.accountType;
                user.createdDate = DateTime.UtcNow;
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var userRole = await _roleManager.FindByNameAsync(RoleEnum.User.ToString());
                    if (userRole == null)
                        await _roleManager.CreateAsync(new ApplicationRole () { Name = RoleEnum.User.ToString()});
                    var roleResult = await _userManager.AddToRoleAsync(user, RoleEnum.User.ToString());
                    if(roleResult.Succeeded)
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, UserPermission.View));
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);



                    if (await userService.tryLogOnUser(user))
                        return LocalRedirect(returnUrl);
                    else
                        ModelState.AddModelError("", "Failed to Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
