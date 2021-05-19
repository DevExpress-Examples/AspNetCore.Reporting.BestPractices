using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.Common.Services;
using AspNetCore.Reporting.Common.Services.Reporting;
using AspNetCore.Reporting.MVC.Data;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Reporting.MVC {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<SchoolDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.ConfigureApplicationCookie(x => {
                x.LoginPath = new PathString("/Account/Login");
            });

            services.AddControllersWithViews().RemoveDefaultReportingControllers().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            services.AddDevExpressControls();
            services.ConfigureReportingServices(x => x.ConfigureReportDesigner(reportDesignerConfigurator => {
                reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<CustomObjectDataSourceWizardTypeProvider>();
            }));
            ServiceRegistrator.AddCommonServices(services, WebHostEnvironment.ContentRootPath);

            services.AddSingleton<IScopedDbContextProvider<SchoolDbContext>, ScopedDbContextProvider<SchoolDbContext>>();
            services.AddScoped<IAuthenticatiedUserService, UserService<SchoolDbContext>>();
            services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension<SchoolDbContext>>();
            services.AddTransient<IUserEmailStore<StudentIdentity>, CustomStudentsUserStore>();
            services.AddTransient<CourseListReportRepository>();
            services.AddTransient<MyEnrollmentsReportRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            app.UseDevExpressControls();

            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            LoggerService.Initialize(new CustomReportingLoggerService(loggerFactory.CreateLogger("DXReporting")));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
