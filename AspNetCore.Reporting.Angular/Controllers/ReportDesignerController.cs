using System.Collections.Generic;
using System.Net.Mime;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Reporting.Common.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReportDesignerSetupController : ControllerBase {
        [HttpPost("[action]")]
        public object GetReportDesignerModel([FromForm] string reportUrl,
            [FromServices] IReportDesignerModelBuilder reportDesignerModel,
            [FromServices] IReportDesignerClientSideModelGenerator modelGenerator) {
            Dictionary<string, object> dataSources = new Dictionary<string, object>();
            //Fill a data source set if needed
            reportDesignerModel
                .Report(reportUrl)
                .DataSources(dataSources)
                .DesignerUri("/DXXRDAngular")
                .ViewerUri("/DXXRDVAngular")
                .QueryBuilderUri("/DXXQBAngular")
                .BuildJsonModel();
            var model = reportDesignerModel.BuildModel();
            var modelJson = modelGenerator.GetJsonModelScript(model);
            return Content(modelJson, MediaTypeNames.Application.Json);
        }
    }
}
