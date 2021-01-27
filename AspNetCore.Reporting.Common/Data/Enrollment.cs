namespace AspNetCore.Reporting.Common.Data {
    public enum Grade {
        A, B, C, D, F
    }

    public class Enrollment {
        public int EnrollmentID { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public StudentIdentity Student { get; set; }
    }
}
