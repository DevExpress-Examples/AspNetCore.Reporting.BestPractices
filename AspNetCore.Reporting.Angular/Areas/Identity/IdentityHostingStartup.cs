using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(AspNetCore.Reporting.Angular.Areas.Identity.IdentityHostingStartup))]
namespace AspNetCore.Reporting.Angular.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
