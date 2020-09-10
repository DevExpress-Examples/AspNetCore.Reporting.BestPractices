using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Reporting.Common.Services {
    public class UserService<T> : IAuthenticatiedUserService where T : DbContext, IStudentEntityProvider {
        readonly IHttpContextAccessor contextAccessor;
        readonly T userEntityProvider;

        public UserService(IHttpContextAccessor contextAccessor, T userEntityProvider) {
            this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            this.userEntityProvider = userEntityProvider ?? throw new ArgumentNullException(nameof(userEntityProvider));
        }

        public string GetCurrentUserId() {
            var sidStr = contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);        
            return sidStr?.Value;
        }

        public IEnumerable<Claim> GetCurrentUserClaims() {
            return contextAccessor.HttpContext.User?.Claims ?? Enumerable.Empty<Claim>();
        }

        public string GetCurrentUserName() {
            return contextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
        }

        public async Task<StudentDetailsModel> AuthenticateAsync(LoginRequest loginRequest) {
            var student = await userEntityProvider.Students.FirstOrDefaultAsync(x => x.Id == loginRequest.UserID);
            if(student == null)
                return null;
            return GetStudentModel(student);
        }

        public IEnumerable<StudentDetailsModel> GetStudentList() {
            return userEntityProvider.Students.Select(GetStudentModel);
        }

        StudentDetailsModel GetStudentModel(StudentIdentity student) {
            return new StudentDetailsModel {
                StudentID = student.Id,
                FirstMidName = student.FirstMidName,
                LastName = student.LastName,
                EnrollmentDate = student.EnrollmentDate
            };
        }
    }
}
