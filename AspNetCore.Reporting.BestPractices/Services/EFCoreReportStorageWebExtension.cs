using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Models;

namespace AspNetCoreReportingApp.Services {
    public class EFCoreReportStorageWebExtension : ReportStorageWebExtension {
        private readonly IUserService userService;
        private readonly SchoolContext dBContext;

        public EFCoreReportStorageWebExtension(IUserService userService, SchoolContext dBContext) {
            this.userService = userService;
            this.dBContext = dBContext;
        }


        public override bool CanSetData(string url) {
            return true;
        }

        public override bool IsValidUrl(string url) {
            return true;
        }

        public override byte[] GetData(string url) {
            var userIdentity = userService.GetCurrentUserId();
            var reportData = dBContext.Reports.Where(a => a.ID == int.Parse(url) && a.Student.ID == userIdentity).FirstOrDefault();
            if(reportData != null) {
                return reportData.ReportLayout;
            } else {
                throw new DevExpress.XtraReports.Web.ClientControls.FaultException(string.Format("Could not find report '{0}'.", url));
            }
        }

        public override Dictionary<string, string> GetUrls() {
            var userIdentity = userService.GetCurrentUserId();
            var reportData = dBContext.Reports.Where(a => a.Student.ID == userIdentity).Select(a => new ReportingControlModel() { Id = a.ID.ToString(), Title = string.IsNullOrEmpty(a.DisplayName) ? "Noname Report" : a.DisplayName });
            var reports = reportData.ToList();
            return reports.ToDictionary(x => x.Id.ToString(), y => y.Title);
        }

        public override void SetData(XtraReport report, string url) {
            var userIdentity = userService.GetCurrentUserId();
            var reportEntity = dBContext.Reports.Where(a => a.ID == int.Parse(url) && a.Student.ID == userIdentity).FirstOrDefault();
            reportEntity.ReportLayout = ReportToByteArray(report);
            reportEntity.DisplayName = report.DisplayName;
            dBContext.SaveChanges();
        }

        public override string SetNewData(XtraReport report, string defaultUrl) {
            var userIdentity = userService.GetCurrentUserId();
            var user = dBContext.Students.Find(userIdentity);
            var newReport = new Report() { DisplayName = defaultUrl, ReportLayout = ReportToByteArray(report), Student = user };
            dBContext.Reports.Add(newReport);
            dBContext.SaveChanges();
            return newReport.ID.ToString();
        }

        static byte[] ReportToByteArray(XtraReport report) {
            using(var memoryStream = new MemoryStream()) {
                report.SaveLayoutToXml(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
