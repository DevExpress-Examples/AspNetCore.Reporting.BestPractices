using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreReportingApp.Auth {
    public class AuthenticationParameters {
        public const string Issuer = "CurrentAuthServer";
        public const string Audience = "CustomTokenClient";
        const string StrongKey = "8F918557-2543-41E0-9D51-B8D89D6C4C50!a51493e7-aff2-4ca5-a994-e6707fcbf392?06DBDAA0-2339-4D48-BD4C-8E49127ED137";
        public const int TokenTimeToLife = 1;
        public static SymmetricSecurityKey GetSymmetricSecurityKey() {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(StrongKey));
        }
    }
}
