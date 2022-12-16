using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AspNetCore.Reporting.Common.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AspNetCore.Reporting.Angular.Areas.Identity.Pages.Account {
    [AllowAnonymous]
    public class LoginModel : PageModel {
        private readonly UserManager<StudentIdentity> _userManager;
        private readonly SignInManager<StudentIdentity> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<StudentIdentity> signInManager,
            ILogger<LoginModel> logger,
            UserManager<StudentIdentity> userManager) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel {
            //[Required]
            //[EmailAddress]
            //public string Email { get; set; }

            [Required]
            public string UserId { get; set; }

            //[Required]
            //[DataType(DataType.Password)]
            //public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null) {
            if(!string.IsNullOrEmpty(ErrorMessage)) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            Users = _userManager.Users.Select(x => new SelectListItem(x.FirstMidName + " " + x.LastName, x.Id));
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            returnUrl = returnUrl ?? Url.Content("~/");

            if(ModelState.IsValid) {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByIdAsync(Input.UserId);
                await _signInManager.SignInAsync(user, Input.RememberMe);
                var userClaims = await _userManager.GetClaimsAsync(user);
                string userName = $"{user.FirstMidName} {user.LastName}";
                userClaims.Add(new Claim(ClaimTypes.Name, userName));
                userClaims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
                userClaims.Add(new Claim(ClaimTypes.Role, "User"));
                userClaims.Add(new Claim(ClaimTypes.Sid, user.Id));
                var identityResult1 = await _userManager.AddClaimsAsync(user, userClaims);

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, userName),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.Sid, user.Id)
                };
                var identityResult2 = await _userManager.AddClaimsAsync(user, claims);

                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);

                //var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                //if(result.Succeeded) {
                //    _logger.LogInformation("User logged in.");
                //    return LocalRedirect(returnUrl);
                //}
                //if(result.RequiresTwoFactor) {
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                //}
                //if(result.IsLockedOut) {
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToPage("./Lockout");
                //} else {
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return Page();
                //}
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
