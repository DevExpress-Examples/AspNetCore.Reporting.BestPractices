using System;
using System.Collections.Concurrent;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace AspNetCoreReportingApp.Services {
    class DocumentViewerAuthorizationService : WebDocumentViewerOperationLogger, IWebDocumentViewerAuthorizationService {
        static ConcurrentDictionary<string, int> DocumentIdOwnerMap { get; } = new ConcurrentDictionary<string, int>();
        static ConcurrentDictionary<string, int> ReportIdOwnerMap { get; } = new ConcurrentDictionary<string, int>();

        IUserService UserService { get; }

        public DocumentViewerAuthorizationService(IUserService userService) {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public override void ReportOpening(string reportId, string documentId, XtraReport report) {
            MapIdentifiersToUser(UserService.GetCurrentUserId(), documentId, reportId);
            base.ReportOpening(reportId, documentId, report);
        }

        public override void BuildStarted(string reportId, string documentId, ReportBuildProperties buildProperties) {
            MapIdentifiersToUser(UserService.GetCurrentUserId(), documentId, reportId);
            base.BuildStarted(reportId, documentId, buildProperties);
        }

        void MapIdentifiersToUser(int userId, string documentId, string reportId) {
            if(!string.IsNullOrEmpty(documentId)) {
                DocumentIdOwnerMap.TryAdd(documentId, userId);
            }
            if(!string.IsNullOrEmpty(reportId)) {
                ReportIdOwnerMap.TryAdd(reportId, userId);
            }
        }

        #region IWebDocumentViewerAuthorizationService
        public bool CanCreateDocument() {
            return true;
        }

        public bool CanCreateReport() {
            return true;
        }

        public bool CanReadDocument(string documentId) {
            return DocumentIdOwnerMap.TryGetValue(documentId, out var ownerId) && ownerId == UserService.GetCurrentUserId();
        }

        public bool CanReadReport(string reportId) {
            return ReportIdOwnerMap.TryGetValue(reportId, out var ownerId) && ownerId == UserService.GetCurrentUserId();
        }

        public bool CanReleaseDocument(string documentId) {
            return true;
        }

        public bool CanReleaseReport(string reportId) {
            return true;
        }
        #endregion
    }
}
