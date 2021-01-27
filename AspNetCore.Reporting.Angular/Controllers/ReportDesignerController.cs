using System.Collections.Generic;
using DevExpress.Compatibility.System.Web;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Reporting.Common.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReportDesignerSetupController : ControllerBase {
        [HttpPost("[action]")]
        public object GetReportDesignerModel([FromForm] string reportUrl) {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            //Fill a data source set if needed
            var modelGenerator = new ReportDesignerClientSideModelGenerator(HttpContext.RequestServices);
            var model = modelGenerator.GetModel(reportUrl, dataSources, "/DXXRDAngular", "/DXXRDVAngular", "/DXXQBAngular");
            string modelJsonScript = modelGenerator.GetJsonModelScript(model);
            return new JavaScriptSerializer().Deserialize<object>(modelJsonScript);
        }
    }
}
