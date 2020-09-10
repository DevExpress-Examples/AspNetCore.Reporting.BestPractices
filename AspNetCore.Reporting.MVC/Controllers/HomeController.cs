using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.Common.Models;
using AspNetCore.Reporting.Common.Services;
using AspNetCore.Reporting.MVC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.MVC.Controllers {
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize]
    public class HomeController : Controller {
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromServices] IAuthenticatiedUserService userService, [FromServices] SchoolDbContext dBContext, [FromServices] UserManager<StudentIdentity> userManager) {
            //var students = DbDefaultsGenerator.GenerateStudents(userManager);
            //foreach(StudentIdentity s in students) {
            //    s.UserName = s.FirstMidName + s.LastName;
            //    s.NormalizedUserName = userManager.NormalizeName(s.UserName);
            //    s.Email = s.UserName + "@sample.email";
            //    s.NormalizedEmail = userManager.NormalizeEmail(s.Email);
            //    s.EmailConfirmed = true;
            //    s.LockoutEnabled = true;
            //    dBContext.Students.Add(s);
            //    var result = await userManager.CreateAsync(s, "Admin@123");
            //    if(result.Succeeded)
            //        dBContext.SaveChanges();
            //    //s.PasswordHash = userManager.PasswordHasher.HashPassword(s, "Admin@123");
            //    //s.SecurityStamp = Guid.NewGuid().ToString("N");
            //}
            //dBContext.SaveChanges();
            //dBContext.Database.EnsureCreated();
            var reportData = !User.Identity.IsAuthenticated 
                ? Enumerable.Empty<ReportingControlModel>()
                : await dBContext
                    .Reports
                    .Where(a => a.Student.Id == userService.GetCurrentUserId())
                    .Select(a => new ReportingControlModel {
                        Id = a.ID.ToString(),
                        Title = string.IsNullOrEmpty(a.DisplayName) ? "Noname Report" : a.DisplayName
                    })
                    .ToListAsync();
            return View(reportData);
        }

        public IActionResult DesignReport(ReportingControlModel model, [FromServices] IAuthenticatiedUserService userService) {
            return View(model);
        }

        public IActionResult DisplayReport(ReportingControlModel model, [FromServices] IAuthenticatiedUserService userService) {
            return View(model);
        }
        
        public IActionResult Privacy() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveReport([FromServices] IAuthenticatiedUserService userService, [FromServices] SchoolDbContext dBContext, int reportId) {
            var userIdentity = userService.GetCurrentUserId();
            var reportData = await dBContext.Reports.Where(a => a.ID == reportId && a.Student.Id == userIdentity).FirstOrDefaultAsync();
            if(reportData != null) {
                dBContext.Reports.Remove(reportData);
                await dBContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
