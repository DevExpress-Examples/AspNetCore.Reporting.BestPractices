using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreReportingApp.Auth;
using System;

namespace AspNetCoreReportingApp {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {
            Configuration = configuration;
            Env = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidIssuer = AuthenticationParameters.Issuer,
                        ValidAudience = AuthenticationParameters.Audience,
                        IssuerSigningKey = AuthenticationParameters.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddDevExpressControls();
            services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var builder = services
                .AddMvc()
                .RemoveDefaultReportingControllers()    // NOTE: make sure the default document viewer controller is not registered
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
#if DEBUG
            if(Env.IsDevelopment()) {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner((reportDesignerConfigurator) => {
                    reportDesignerConfigurator.RegisterObjectDataSourceConstructorFilterService<ObjectDataSourceConstructorFilterService>();
                    reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<ObjectDataSourceWizardTypeProvider>();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            services.AddScoped<IWebDocumentViewerAuthorizationService, DocumentViewerAuthorizationService>();
            services.AddScoped<WebDocumentViewerOperationLogger, DocumentViewerAuthorizationService>();

            services.AddSingleton<IScopedDbContextProvider<SchoolContext>, ScopedDbContextProvider<SchoolContext>>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWebDocumentViewerReportResolver, WebDocumentViewerReportResolver>();
            services.AddScoped<IObjectDataSourceInjector, ObjectDataSourceInjector>();
            services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension>();
            services.AddTransient<CourseListReportRepository>();
            services.AddTransient<MyEnrollmentsReportRepository>();
            services.AddScoped<PreviewReportCustomizationService, CustomPreviewReportCustomizationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var reportingLogger = loggerFactory.CreateLogger("DXReporting");
            DevExpress.XtraReports.Web.ClientControls.LoggerService.Initialize((exception, message) => {
                var logMessage = $"[{DateTime.Now}]: Exception occurred. Message: '{message}'. Exception Details:\r\n{exception}";
                reportingLogger.LogError(logMessage);
            });

            app.UseDevExpressControls();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}