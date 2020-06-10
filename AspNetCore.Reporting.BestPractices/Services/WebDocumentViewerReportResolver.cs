using System;
using System.IO;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace AspNetCoreReportingApp.Services {
    class WebDocumentViewerReportResolver : IWebDocumentViewerReportResolver {
        IObjectDataSourceInjector DataSourceInjector { get; }
        ReportStorageWebExtension ReportStorageWebExtension { get; }

        public WebDocumentViewerReportResolver(ReportStorageWebExtension reportStorageWebExtension, IObjectDataSourceInjector dataSourceInjector) {
            DataSourceInjector = dataSourceInjector ?? throw new ArgumentNullException(nameof(dataSourceInjector));
            ReportStorageWebExtension = reportStorageWebExtension ?? throw new ArgumentNullException(nameof(reportStorageWebExtension));
        }

        public XtraReport Resolve(string reportEntry) {
            using(MemoryStream ms = new MemoryStream(ReportStorageWebExtension.GetData(reportEntry))) {
                var report = XtraReport.FromStream(ms);
                DataSourceInjector.Process(report);
                return report;
            }
        }
    }
}
