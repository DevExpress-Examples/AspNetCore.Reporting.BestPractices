using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.MVC.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Reporting.MVC {
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
                    var dbContext = services.GetRequiredService<SchoolDbContext>();
                    var userManager = services.GetRequiredService<UserManager<StudentIdentity>>();
                    DbInitializer.Initialize(dbContext, userManager, new Common.Reports.ReportsFactory());
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
