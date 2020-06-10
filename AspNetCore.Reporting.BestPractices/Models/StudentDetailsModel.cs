using System;

namespace AspNetCoreReportingApp.Models {
    public class StudentDetailsModel {
        public int StudentID { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
