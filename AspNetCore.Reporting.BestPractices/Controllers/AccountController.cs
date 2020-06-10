using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Models;

namespace AspNetCoreReportingApp.Controllers {
    public class AccountController : Controller {

        [HttpGet]
        public async Task<IActionResult> Login([FromServices]SchoolContext dbContext) {
            return View(await GetLoginScreenModelAsync(dbContext));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromServices]SchoolContext dbContext, int userId, string returnUrl) {
            var student = await dbContext.Students.FindAsync(userId);
            if(student != null) {
                await SignIn(student);

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

        async Task SignIn(Student user) {
            string userName = $"{user.FirstMidName} {user.LastName}";

            var claims = new[] {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.Sid, user.ID.ToString(CultureInfo.InvariantCulture))
            };

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });
        }

        async Task<LoginScreenModel> GetLoginScreenModelAsync(SchoolContext dBContext) {
            var model = new LoginScreenModel();
            model.Users = await dBContext.Students
                .Select(x => new SelectListItem {
                    Value = x.ID.ToString(CultureInfo.InvariantCulture),
                    Text = $"{x.FirstMidName} {x.LastName}"
                })
                .ToListAsync();
            return model;
        }

    }
}
