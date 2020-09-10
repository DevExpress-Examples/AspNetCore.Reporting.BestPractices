using System;
using DevExpress.XtraReports.Web.ClientControls;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Reporting.Common.Services {
    public class CustomReportingLoggerService : LoggerService {
        private readonly ILogger logger;

        public CustomReportingLoggerService(ILogger logger) {
            this.logger = logger;
        }
        public override void Error(Exception exception, string message) {
            logger.LogError($"[{DateTime.Now}] Reporting. Exception occurred. Message: '{message}'. Exception Details:\r\n{ex}");
        }
        public override void Info(string message) {
            logger.LogInformation($"[{DateTime.Now}] Reporting. Message: '{message}'.");
        }
    }
}
