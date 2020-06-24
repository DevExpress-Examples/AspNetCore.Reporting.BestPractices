using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AspNetCoreReportingApp.Auth;
using AspNetCoreReportingApp.Models;
using AspNetCoreReportingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreReportingApp.Controllers {
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AccountApiController : ControllerBase {
        IAuthenticatiedUserService userService;
        public AccountApiController(IAuthenticatiedUserService userService) {
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest loginModel) {
            var response = await userService.AuthenticateAsync(loginModel);
            //authenticate user by login model from the client

            if(response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var token = CreateJwtBearerToken();
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetUserList() {
            var students = userService.GetStudentList();
            return Ok(students);
        }

        string CreateJwtBearerToken() {
            var claims = userService.GetCurrentUserClaims();
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                        issuer: AuthenticationParameters.Issuer,
                        audience: AuthenticationParameters.Audience,
                        notBefore: now,
                        claims: claims,
                        expires: now.Add(TimeSpan.FromDays(AuthenticationParameters.TokenTimeToLife)),
                        signingCredentials: new SigningCredentials(AuthenticationParameters.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                    );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedToken;
        }
    }
}
