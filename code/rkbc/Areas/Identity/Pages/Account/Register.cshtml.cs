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
using System.Globalization;
using rkbc.core.models;
using rkbc.web.viewmodels;
using rkbc.core.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using rkbc.web.helpers;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

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
        private readonly IStringLocalizer<RegisterModel> Localizer;
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<RegisterModel> logger,
            UserService _userService,
            IStringLocalizer<RegisterModel> _localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            userService = _userService;
            Localizer = _localizer;
            Input = new InputModel();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public List<SelectListItem> countryOptions { get; set; }
        public List<SelectListItem> accountTypeOptions { get; set; }

        public bool LookupEmail(string email)
        {
           var user = _userManager.FindByEmailAsync(email);
            return user == null ? true : false;
        }
        public void OnGet(string returnUrl = null)
        {
            var names = Enum.GetNames(typeof(CountryEnum));
            var countryList = new List<SelectListItem>();
            for (int i = 0; i < names.Length; i++)
            {
                var value = (CountryEnum)Enum.Parse(typeof(CountryEnum), names[i]);
                countryList.Add(new SelectListItem{ Value = ((int)value).ToString(), Text = Localizer[EnumHelper.GetDisplayName(value)] });
            }
            countryOptions = countryList;
            var accountNames = Enum.GetNames(typeof(AccountType));
            var accountTypeList = new List<SelectListItem>();
            for (int i = 0; i < accountNames.Length; i++)
            {
                var value = (AccountType)Enum.Parse(typeof(AccountType), accountNames[i]);
                accountTypeList.Add(new SelectListItem { Value = ((int)value).ToString(), Text = Localizer[EnumHelper.GetDisplayName(value)]});
            }
            accountTypeOptions = accountTypeList;

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
