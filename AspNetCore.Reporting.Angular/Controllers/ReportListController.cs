using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting.Angular.Data;
using AspNetCore.Reporting.Common.Models;
using AspNetCore.Reporting.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.Common.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReportListController : ControllerBase {
        [HttpGet]
        public async Task<IEnumerable<ReportingControlModel>> Get([FromServices] SchoolDbContext dbContext, [FromServices] IAuthenticatiedUserService userService) {
            var reportData = !User.Identity.IsAuthenticated
                ? Enumerable.Empty<ReportingControlModel>()
                : await dbContext
                    .Reports
                    .Where(a => a.Student.Id == userService.GetCurrentUserId())
                    .Select(a => new ReportingControlModel {
                        Id = a.ID.ToString(),
                        Title = string.IsNullOrEmpty(a.DisplayName) ? "Noname Report" : a.DisplayName
                    })
                    .ToListAsync();
            return reportData;
        }
    }
}
