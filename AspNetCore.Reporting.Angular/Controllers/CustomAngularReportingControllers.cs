using System.Threading.Tasks;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Reporting.Common.Controllers {
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("DXXRDVAngular")]
    public class AngularWebDocumentViewerController : WebDocumentViewerController {
        public AngularWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService) {
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("DXXQBAngular")]
    public class AngularQueryBuilderController : QueryBuilderController {
        public AngularQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService) {
        }
    }

    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("DXXRDAngular")]
    public class AngularReportDesignerController : ReportDesignerController {
        public AngularReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(controllerService) {
        }
        
        public override Task<IActionResult> Invoke() {
            return base.Invoke();
        }
    }
}
