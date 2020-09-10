using AspNetCoreReportingApp.Data;

namespace AspNetCoreReportingApp.Models {
    public class LoginResponse {
        public string Id { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }

        public LoginResponse(StudentIdentity student, string token) {
            Id = student.Id;
            FirstMidName = student.FirstMidName;
            LastName = student.LastName;
            Token = token;
        }
    }
}
