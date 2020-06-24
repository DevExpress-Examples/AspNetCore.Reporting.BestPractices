using AspNetCoreReportingApp.Data;

namespace AspNetCoreReportingApp.Models {
    public class LoginResponse {
        public int Id { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }

        public LoginResponse(Student student, string token) {
            Id = student.ID;
            FirstMidName = student.FirstMidName;
            LastName = student.LastName;
            Token = token;
        }
    }
}
