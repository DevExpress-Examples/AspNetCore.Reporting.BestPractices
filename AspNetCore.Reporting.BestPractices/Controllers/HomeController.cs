using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreReportingApp.Auth;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Models;
using AspNetCoreReportingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreReportingApp.Controllers {
    [Authorize]
    public class HomeController : Controller {

        public async Task<IActionResult> Index([FromServices] IUserService userService, [FromServices] SchoolContext dBContext) {
            var reportData = dBContext.Reports.Where(a => a.Student.ID == userService.GetCurrentUserId()).Select(a => new ReportingControlModel() { Id = a.ID.ToString(), Title = string.IsNullOrEmpty(a.DisplayName) ? "Noname Report" : a.DisplayName });
            return View(await reportData.ToListAsync());
        }

        public IActionResult DesignReport(ReportingControlModel model, [FromServices] IUserService userService) {
            model.BearerToken = CreateJwtBearerToken(userService);
            return View(model);
        }

        public IActionResult DisplayReport(ReportingControlModel model, [FromServices] IUserService userService) {
            model.BearerToken = CreateJwtBearerToken(userService);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveReport([FromServices] IUserService userService, [FromServices] SchoolContext dBContext, int reportId) {
            var userIdentity = userService.GetCurrentUserId();
            var reportData = await dBContext.Reports.Where(a => a.ID == reportId && a.Student.ID == userIdentity).FirstOrDefaultAsync();
            if(reportData != null) {
                dBContext.Reports.Remove(reportData);
                await dBContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        string CreateJwtBearerToken(IUserService userService) {
            var claims = userService.GetCurrentUserClaims();
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                        issuer: AuthenticationParameters.Issuer,
                        audience: AuthenticationParameters.Audience,
                        notBefore: now,
                        claims: claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthenticationParameters.TokenTimeToLife)),
                        signingCredentials: new SigningCredentials(AuthenticationParameters.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                    );
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedToken;
        }
    }
}
