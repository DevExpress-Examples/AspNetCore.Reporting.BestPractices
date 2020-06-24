using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreReportingApp.Models;

namespace AspNetCoreReportingApp.Services {
    public interface IAuthenticatiedUserService {
        int GetCurrentUserId();
        string GetCurrentUserName();
        IEnumerable<Claim> GetCurrentUserClaims();
        Task<StudentDetailsModel> AuthenticateAsync(LoginRequest loginRequest);
        IEnumerable<StudentDetailsModel> GetStudentList();
    }
}
