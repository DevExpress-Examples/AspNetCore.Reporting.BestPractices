using System.Threading.Tasks;
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreReportingApp.Controllers {
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("DXXRDVMVC")]
    public class CustomMVCWebDocumentViewerController : WebDocumentViewerController {
        public CustomMVCWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService) {
        }

        public override Task<IActionResult> Invoke() {
            return base.Invoke();
        }
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("DXXQBMVC")]
    public class CustomMVCQueryBuilderController : QueryBuilderController {
        public CustomMVCQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService) {
        }
        public override Task<IActionResult> Invoke() {
            return base.Invoke();
        }
    }

    
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("DXXRDMVC")]
    public class CustomMVCReportDesignerController : ReportDesignerController {
        public CustomMVCReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(controllerService) {
        }
        
        public override Task<IActionResult> Invoke() {
            return base.Invoke();
        }
    }
}
