using System;
using System.Collections.Generic;
using AspNetCore.Reporting.Angular.Services;
using DevExpress.DataAccess.Web;

namespace AspNetCore.Reporting.Common.Services.Reporting {
    public class CustomObjectDataSourceWizardTypeProvider : IObjectDataSourceWizardTypeProvider {
        public IEnumerable<Type> GetAvailableTypes(string context) {
            return new[] {
                typeof(CourseListReportRepository),
                typeof(MyEnrollmentsReportRepository)
            };
        }
    }
}
