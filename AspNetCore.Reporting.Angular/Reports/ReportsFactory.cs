using System;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;

namespace AspNetCore.Reporting.Common.Reports {
    public class ReportsFactory {
        public Dictionary<string, Func<XtraReport>> Reports {
            get {
                return new Dictionary<string, Func<XtraReport>>() {
                    ["Enrollments"] = () => new MyEnrollmentsReport(),
                    ["CourseList"] = () => new CourseListReport(),
                };
            }
        }
    }
}
