using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreReportingApp.Services {
    public class UserService : IAuthenticatiedUserService {
        readonly IHttpContextAccessor contextAccessor;
        readonly SchoolContext dbContext;

        public UserService(IHttpContextAccessor contextAccessor, SchoolContext dbContext) {
            this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public int GetCurrentUserId() {
            var sidStr = contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid);        
            return Convert.ToInt32(sidStr.Value, CultureInfo.InvariantCulture);
        }

        public IEnumerable<Claim> GetCurrentUserClaims() {
            return contextAccessor.HttpContext.User?.Claims ?? Enumerable.Empty<Claim>();
        }

        public string GetCurrentUserName() {
            return contextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
        }

        public async Task<StudentDetailsModel> AuthenticateAsync(LoginRequest loginRequest) {
            var student = await dbContext.Students.FirstOrDefaultAsync(x => x.ID == loginRequest.UserID);
            if(student == null)
                return null;
            return GetStudentModel(student);
        }

        public IEnumerable<StudentDetailsModel> GetStudentList() {
            return dbContext.Students.Select(GetStudentModel);
        }

        StudentDetailsModel GetStudentModel(Student student) {
            return new StudentDetailsModel {
                StudentID = student.ID,
                FirstMidName = student.FirstMidName,
                LastName = student.LastName,
                EnrollmentDate = student.EnrollmentDate
            };
        }
    }
}
