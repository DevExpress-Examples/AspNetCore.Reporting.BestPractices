using System.Collections.Generic;
using DevExpress.Compatibility.System.Web;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreReportingApp.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReportDesignerSetupController : ControllerBase {
        [HttpPost("[action]")]
        public object GetReportDesignerModel([FromForm] string reportUrl) {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            SqlDataSource ds = new SqlDataSource("NWindConnectionString");

            // Create a SQL query to access the Products data table.
            SelectQuery query = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumnsFromTable().Build("Products");
            ds.Queries.Add(query);
            ds.RebuildResultSchema();
            dataSources.Add("Northwind", ds);

            string modelJsonScript = new ReportDesignerClientSideModelGenerator(HttpContext.RequestServices).GetJsonModelScript(reportUrl, dataSources, "/DXXRDAngular", "/DXXRDVAngular", "/DXXQBAngular");
            return new JavaScriptSerializer().Deserialize<object>(modelJsonScript);
        }
    }
}
