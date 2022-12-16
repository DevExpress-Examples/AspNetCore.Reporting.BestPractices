using System;
using AspNetCore.Reporting.Angular.Data;
using AspNetCore.Reporting.Common.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Reporting.Angular {
    public class Program {
        public static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            InitializeDb(host);
            host.Run();
        }
        static void InitializeDb(IHost host) {
            using(var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try {
                    var context = services.GetRequiredService<SchoolDbContext>();
                    var userManager = services.GetRequiredService<UserManager<StudentIdentity>>();
                    DbInitializer.Initialize(context, userManager, new Common.Reports.ReportsFactory());
                } catch(Exception exception) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "An error occurred while seeding the database.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
