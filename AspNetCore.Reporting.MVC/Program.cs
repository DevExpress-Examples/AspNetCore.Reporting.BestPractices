using AspNetCore.Reporting.MVC.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.Reporting.MVC {
    public class Program {
        public static void Main(string[] args) {
            var host = CreateHostBuilder(args).Build();
            InitializeDb(host);
            host.Run();
        }

        static void InitializeDb(IHost host) {
            using(var scope = host.Services.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
                DbInitializer.Initialize(dbContext, new Common.Reports.ReportsFactory());
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
