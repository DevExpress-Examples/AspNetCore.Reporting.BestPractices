using System;
using System.Collections.Generic;
using DevExpress.DataAccess.Web;

namespace AspNetCore.Reporting.Common.Services {
    public class CustomObjectDataSourceWizardTypeProvider : IObjectDataSourceWizardTypeProvider {
        public IEnumerable<Type> GetAvailableTypes(string context) {
            return new[] {
                typeof(CourseListReportRepository),
                typeof(MyEnrollmentsReportRepository)
            };
        }
    }
}
