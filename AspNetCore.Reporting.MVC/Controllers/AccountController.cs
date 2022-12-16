using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.MVC.Controllers;
using AspNetCore.Reporting.MVC.Data;
using AspNetCore.Reporting.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConnectionProviderSample.Controllers {
    public class AccountController : Controller {

        [HttpGet]
        public async Task<IActionResult> Login([FromServices] SchoolDbContext dbContext) {
            return View(await GetLoginScreenModelAsync(dbContext));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromServices] SchoolDbContext dbContext, string userId, string returnUrl) {
            var user = await dbContext.Users.FindAsync(userId);
            if(user != null) {
                await SignIn(user);

                if(Url.IsLocalUrl(returnUrl)) {
                    return Redirect(returnUrl);
                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            throw new SecurityException($"User not found by the ID: {userId}");
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        async Task SignIn(StudentIdentity user) {
            string userName = $"{user.FirstMidName} {user.LastName}";

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.Sid, user.Id)
            };

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });
        }

        async Task<LoginScreenModel> GetLoginScreenModelAsync(SchoolDbContext dBContext) {
            var model = new LoginScreenModel();
            model.Users = await dBContext.Users
                .Select(x => new SelectListItem {
                    Value = x.Id,
                    Text = $"{x.FirstMidName} {x.LastName}"
                })
                .ToListAsync();
            return model;
        }

    }
}
