using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner.Services;

namespace AspNetCoreReportingApp.Services.Reporting {
    public class CustomPreviewReportCustomizationService : PreviewReportCustomizationService {
        readonly IObjectDataSourceInjector objectDataSourceInjector;
        public CustomPreviewReportCustomizationService(IObjectDataSourceInjector objectDataSourceInjector) {
            this.objectDataSourceInjector = objectDataSourceInjector;

        }
        public override void CustomizeReport(XtraReport report) {
            objectDataSourceInjector.Process(report);
        }
    }
}
