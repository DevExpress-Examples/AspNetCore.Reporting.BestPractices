using System;
using System.Collections.Concurrent;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace AspNetCore.Reporting.Common.Services.Reporting {
    class DocumentViewerAuthorizationService : WebDocumentViewerOperationLogger, IWebDocumentViewerAuthorizationService, IExportingAuthorizationService {
        static ConcurrentDictionary<string, string> DocumentIdOwnerMap { get; } = new ConcurrentDictionary<string, string>();
        static ConcurrentDictionary<string, string> ExportedDocumentIdOwnerMap { get; } = new ConcurrentDictionary<string, string>();
        static ConcurrentDictionary<string, string> ReportIdOwnerMap { get; } = new ConcurrentDictionary<string, string>();

        IAuthenticatiedUserService UserService { get; }

        public DocumentViewerAuthorizationService(IAuthenticatiedUserService userService) {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public override void ReportOpening(string reportId, string documentId, XtraReport report) {
            MapIdentifiersToUser(UserService.GetCurrentUserId(), documentId, reportId, null);
            base.ReportOpening(reportId, documentId, report);
        }

        public override void BuildStarted(string reportId, string documentId, ReportBuildProperties buildProperties) {
            MapIdentifiersToUser(UserService.GetCurrentUserId(), documentId, reportId, null);
            base.BuildStarted(reportId, documentId, buildProperties);
        }

        public override ExportedDocument ExportDocumentStarting(string documentId, string asyncExportOperationId, string format, ExportOptions options, PrintingSystemBase printingSystem, Func<ExportedDocument> doExportSynchronously) {
            return base.ExportDocumentStarting(documentId, asyncExportOperationId, format, options, printingSystem, doExportSynchronously);
        }

        void MapIdentifiersToUser(string userId, string documentId, string reportId, string exportedDocumentId) {
            if(!string.IsNullOrEmpty(exportedDocumentId))
                ExportedDocumentIdOwnerMap.TryAdd(exportedDocumentId, userId);

            if(!string.IsNullOrEmpty(documentId)) 
                DocumentIdOwnerMap.TryAdd(documentId, userId);

            if(!string.IsNullOrEmpty(reportId)) 
                ReportIdOwnerMap.TryAdd(reportId, userId);

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

        public bool CanReadExportedDocument(string exportedDocumentId) {
            return ExportedDocumentIdOwnerMap.TryGetValue(exportedDocumentId, out var ownerId) && ownerId == UserService.GetCurrentUserId();
        }
        #endregion
    }
}
