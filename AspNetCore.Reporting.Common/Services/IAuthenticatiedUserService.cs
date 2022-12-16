using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Models;

namespace AspNetCore.Reporting.Common.Services {
    public interface IAuthenticatiedUserService {
        string GetCurrentUserId();
        string GetCurrentUserName();
        IEnumerable<Claim> GetCurrentUserClaims();
        Task<StudentDetailsModel> AuthenticateAsync(LoginRequest loginRequest);
        IEnumerable<StudentDetailsModel> GetStudentList();
    }
}
