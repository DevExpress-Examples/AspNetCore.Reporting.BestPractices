using System;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace AspNetCoreReportingApp.Services.Reporting {
    public class CustomDocumentViewerExceptionHanlder : WebDocumentViewerExceptionHandler {
        public override string GetExceptionMessage(Exception ex) {
            //: check type of custom exceptions here
            return base.GetExceptionMessage(ex);
        }
    }
}
