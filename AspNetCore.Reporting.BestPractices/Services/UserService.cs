using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreReportingApp.Services {
    public interface IUserService {
        int GetCurrentUserId();
        string GetCurrentUserName();
        IEnumerable<Claim> GetCurrentUserClaims();
    }

    public class UserService : IUserService {
        readonly IHttpContextAccessor contextAccessor;

        public UserService(IHttpContextAccessor contextAccessor) {
            this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
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
    }
}
