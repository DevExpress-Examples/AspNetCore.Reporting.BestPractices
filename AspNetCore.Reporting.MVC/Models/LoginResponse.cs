using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.MVC.Data;

namespace AspNetCore.Reporting.MVC.Models {
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
