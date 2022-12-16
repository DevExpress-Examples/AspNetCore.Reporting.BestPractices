using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Models;
using AspNetCore.Reporting.Common.Services;
using AspNetCore.Reporting.MVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.MVC.Controllers {
    [Authorize]
    public class HomeController : Controller {
        public async Task<IActionResult> Index([FromServices] IAuthenticatiedUserService userService, [FromServices] SchoolDbContext dBContext) {
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

        public IActionResult DesignReport(ReportingControlModel model) {
            return View(model);
        }

        public IActionResult DisplayReport(ReportingControlModel model) {
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
