using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Models;
using AspNetCore.Reporting.Common.Services;
using AspNetCore.Reporting.MVC.Data;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.MVC.Controllers {
    [Authorize]
    public class HomeController : Controller {
        const string QueryBuilderHandlerUri = "/DXXQBMVC";
        const string ReportDesignerHandlerUri = "/DXXRDMVC";
        const string ReportViewerHandlerUri = "/DXXRDVMVC";
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

        public IActionResult DesignReport([FromServices] IReportDesignerClientSideModelGenerator clientSideModelGenerator,
                                          [FromServices] IConnectionProviderService connectionProviderService, ReportingControlModel controlModel) {
            Models.CustomDesignerModel model = new Models.CustomDesignerModel();
            var report = string.IsNullOrEmpty(controlModel.Id) ? new XtraReport() : null;
            model.DesignerModel = CreateReportDesignerModel(clientSideModelGenerator, connectionProviderService, controlModel.Id, report);
            model.Title = controlModel.Title;
            return View(model);
        }

        public IActionResult DisplayReport([FromServices] IWebDocumentViewerClientSideModelGenerator clientSideModelGenerator, ReportingControlModel controlModel) {
            var model = new Models.CustomViewerModel {
                ViewerModel = clientSideModelGenerator.GetModel(controlModel.Id, ReportViewerHandlerUri),
                Title = controlModel.Title
            };
            return View(model);
        }

        public static Dictionary<string, object> GetAvailableDataSources(IConnectionProviderService connectionProviderService) {
            var dataSources = new Dictionary<string, object>();
            // Create a SQL data source with the specified connection string.
            SqlDataSource ds = new SqlDataSource("NWindConnectionString");
            ds.ReplaceService(connectionProviderService, noThrow: true);
            // Create a SQL query to access the Products data table.
            SelectQuery query = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumnsFromTable().Build("Products");
            ds.Queries.Add(query);
            ds.RebuildResultSchema();
            dataSources.Add("Northwind", ds);
            return dataSources;
        }

        public static ReportDesignerModel CreateReportDesignerModel(IReportDesignerClientSideModelGenerator clientSideModelGenerator, IConnectionProviderService connectionProviderService, string reportName, XtraReport report) {
            var dataSources = GetAvailableDataSources(connectionProviderService);
            if(report != null) {
                return clientSideModelGenerator.GetModel(report, dataSources, ReportDesignerHandlerUri, ReportViewerHandlerUri, QueryBuilderHandlerUri);
            }
            return clientSideModelGenerator.GetModel(reportName, dataSources, ReportDesignerHandlerUri, ReportViewerHandlerUri, QueryBuilderHandlerUri);
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
