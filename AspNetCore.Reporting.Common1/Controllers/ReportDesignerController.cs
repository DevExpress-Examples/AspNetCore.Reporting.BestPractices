using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevExpress.Compatibility.System.Web;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreReportingApp.Controllers {
    [Route("api/[controller]")]
    public class ReportDesignerApiController : Controller {
        [HttpPost("[action]"), HttpGet("[action]"), HttpOptions("[action]")]
        public object GetReportList([FromServices] ReportStorageWebExtension reportStorageWebExtension) {
            return reportStorageWebExtension.GetUrls();
        }

        [HttpPost("[action]")]
        public object GetReportDesignerModel([FromForm] string reportUrl) {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            //SqlDataSource ds = new SqlDataSource("NWindConnectionString");

            //// Create a SQL query to access the Products data table.
            //SelectQuery query = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumnsFromTable().Build("Products");
            //ds.Queries.Add(query);
            //ds.RebuildResultSchema();
            //dataSources.Add("Northwind", ds);
            var modelGenerator = new ReportDesignerClientSideModelGenerator(HttpContext.RequestServices);
            var model = modelGenerator.GetModel(reportUrl, dataSources, "/DXXRD", "/DXXRDV", "/DXXQB");
            string modelJsonScript = modelGenerator.GetJsonModelScript(model);

            return new JavaScriptSerializer().Deserialize<object>(modelJsonScript);
        }
    }
}