using System;
using System.IO;
using DevExpress.XtraReports.Web.QueryBuilder.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace AspNetCoreReportingApp.Services.Reporting {
    public class CustomWebDocumentViewerExceptionHandler : WebDocumentViewerExceptionHandler {
        public override string GetExceptionMessage(Exception ex) {
            if(ex is FileNotFoundException) {
#if DEBUG
                return ex.Message;
#else
                return "File is not found.";
#endif
            }
            return base.GetExceptionMessage(ex);
        }
        public override string GetUnknownExceptionMessage(Exception ex) {
            return $"{ex.GetType().Name} occurred. See the log file for more details.";
        }
    }

    public class CustomReportDesignerExceptionHandler : ReportDesignerExceptionHandler {
        public override string GetExceptionMessage(Exception ex) {
            if(ex is FileNotFoundException) {
#if DEBUG
                return ex.Message;
#else
                return "File is not found.";
#endif
            }
            return base.GetExceptionMessage(ex);
        }
        public override string GetUnknownExceptionMessage(Exception ex) {
            return $"{ex.GetType().Name} occurred. See the log file for more details.";
        }
    }
    public class CustomQueryBuilderExceptionHandler : QueryBuilderExceptionHandler {
        public override string GetUnknownExceptionMessage(Exception ex) {
            return $"{ex.GetType().Name} occurred. See the log file for more details.";
        }
    }
}
