using System.Linq;
using AspNetCore.Reporting.Angular.Data;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.Common.Services;
using AspNetCore.Reporting.Common.Services.Reporting;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Reporting.Angular {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDevExpressControls();
            services.AddDbContext<SchoolDbContext>(options =>
                options
                //.UseSqlServer(Configuration.GetConnectionString("DefaultMSSqlConnection")));
                .UseSqlite(Configuration.GetConnectionString("DefaultSqliteConnection")));

            services.AddDefaultIdentity<StudentIdentity>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddUserManager<UserManager<StudentIdentity>>()
                .AddEntityFrameworkStores<SchoolDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<StudentIdentity, SchoolDbContext>(x=> {
                    var api = x.ApiResources.FirstOrDefault();
                    api.UserClaims = new[] { System.Security.Claims.ClaimTypes.Sid };
                });

            services.AddAuthentication()
                .AddIdentityServerJwt();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages();
            services.ConfigureReportingServices(x => x.ConfigureReportDesigner(reportDesignerConfigurator => {
                reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<CustomObjectDataSourceWizardTypeProvider>();
            }));
            ServiceRegistrator.AddCommonServices(services);

            services.AddSingleton<IScopedDbContextProvider<SchoolDbContext>, ScopedDbContextProvider<SchoolDbContext>>();
            services.AddScoped<IAuthenticatiedUserService, UserService<SchoolDbContext>>();
            services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension<SchoolDbContext>>();
            services.AddTransient<CourseListReportRepository>();
            services.AddTransient<MyEnrollmentsReportRepository>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            app.UseDevExpressControls();
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if(!env.IsDevelopment()) {
                app.UseSpaStaticFiles();
            }

            LoggerService.Initialize(new CustomReportingLoggerService(loggerFactory.CreateLogger("DXReporting")));

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if(env.IsDevelopment()) {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
