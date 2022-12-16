using System;
using AspNetCore.Reporting.MVC.Data;
using Microsoft.AspNetCore.Hosting;
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
                    DbInitializer.Initialize(dbContext, new Common.Reports.ReportsFactory());
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
