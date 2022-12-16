using System;

namespace AspNetCore.Reporting.Common.Models {
    public class StudentDetailsModel {
        public string StudentID { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
