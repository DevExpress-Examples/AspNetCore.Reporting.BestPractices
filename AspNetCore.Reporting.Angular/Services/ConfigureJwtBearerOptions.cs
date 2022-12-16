using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
namespace AspNetCore.Reporting.Common.Services {

    public class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions> {
        public void PostConfigure(string name, JwtBearerOptions options) {
            var originalOnMessageReceived = options.Events.OnMessageReceived;
            options.Events.OnMessageReceived = async context => {
                await originalOnMessageReceived(context);

                if(string.IsNullOrEmpty(context.Token) && context.Request.HasFormContentType) {
                    var formData = await context.Request.ReadFormAsync();
                    var accessToken = formData?["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if(!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/DXXRDVAngular")) {
                        context.Token = accessToken;
                    }
                }
            };
        }
    }
}
