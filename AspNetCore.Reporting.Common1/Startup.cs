using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCoreReportingApp.Auth;
using AspNetCoreReportingApp.Data;
using AspNetCoreReportingApp.Services;
using AspNetCoreReportingApp.Services.Reporting;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.QueryBuilder.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using IDataSourceWizardConnectionStringsProvider = DevExpress.DataAccess.Web.IDataSourceWizardConnectionStringsProvider;

namespace AspNetCoreReportingApp {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {
            Configuration = configuration;
            Env = hostingEnvironment;
        }

        public static IConfiguration Configuration { get; set; }
        IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<SchoolDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("AspNetCoreReportingApp_DefaultConnection")));

            services.AddDefaultIdentity<StudentIdentity>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<SchoolDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<StudentIdentity, SchoolDbContext>();

            services
                .AddAuthentication(options => {
                })
                .AddIdentityServerJwt(
                )
                .AddCookie(options => {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Home/Error";
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                })
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidIssuer = AuthenticationParameters.Issuer,
                        ValidAudience = AuthenticationParameters.Audience,
                        IssuerSigningKey = AuthenticationParameters.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                })
                ;

            services.AddAuthorization(options => {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddDevExpressControls();


            var builder = services
                .AddControllersWithViews()
                .RemoveDefaultReportingControllers()    // NOTE: make sure the default document viewer controller is not registered
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
#if DEBUG
            if(Env.IsDevelopment()) {
                builder.AddRazorRuntimeCompilation();
            }
#endif
            services.AddRazorPages();

            var cacheCleanerSettings = new CacheCleanerSettings(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
            services.AddSingleton<CacheCleanerSettings>(cacheCleanerSettings);

            var storageCleanerSettings = new StorageCleanerSettings(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(12), TimeSpan.FromHours(12), TimeSpan.FromHours(12));
            services.AddSingleton<StorageCleanerSettings>(storageCleanerSettings);

            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner((reportDesignerConfigurator) => {
                    reportDesignerConfigurator.RegisterObjectDataSourceConstructorFilterService<CustomObjectDataSourceConstructorFilterService>();
                    reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<CustomObjectDataSourceWizardTypeProvider>();
                    //reportDesignerConfigurator.RegisterDataSourceWizardConnectionStringsProvider<CustomSqlDataSourceWizardConnectionStringsProvider>(true);
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    // StorageSynchronizationMode.InterThread - it is a default value, use InterProcess if you use multiple application instances without ARR Affinity
                    viewerConfigurator.UseFileDocumentStorage("ViewerStorages\\Documents", StorageSynchronizationMode.InterThread);
                    viewerConfigurator.UseFileExportedDocumentStorage("ViewerStorages\\ExportedDocuments", StorageSynchronizationMode.InterThread);
                    viewerConfigurator.UseFileReportStorage("ViewerStorages\\Reports", StorageSynchronizationMode.InterThread);
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            services.AddScoped<IWebDocumentViewerExceptionHandler, CustomWebDocumentViewerExceptionHandler>();
            services.AddScoped<IReportDesignerExceptionHandler, CustomReportDesignerExceptionHandler>();
            services.AddScoped<IQueryBuilderExceptionHandler, CustomQueryBuilderExceptionHandler>();

            services.AddScoped<IWebDocumentViewerAuthorizationService, DocumentViewerAuthorizationService>();
            services.AddScoped<WebDocumentViewerOperationLogger, DocumentViewerAuthorizationService>();

            services.AddSingleton<IScopedDbContextProvider<SchoolDbContext>, ScopedDbContextProvider<SchoolDbContext>>();

            services.AddScoped<IAuthenticatiedUserService, UserService>();
            services.AddScoped<IWebDocumentViewerReportResolver, WebDocumentViewerReportResolver>();
            services.AddScoped<IObjectDataSourceInjector, ObjectDataSourceInjector>();
            services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension>();
            services.AddTransient<CourseListReportRepository>();
            services.AddTransient<MyEnrollmentsReportRepository>();
            services.AddScoped<PreviewReportCustomizationService, CustomPreviewReportCustomizationService>();

            services.AddScoped<IDataSourceWizardConnectionStringsProvider, CustomSqlDataSourceWizardConnectionStringsProvider>();
            services.AddScoped<IConnectionProviderService, CustomConnectionProviderService>();
            services.AddScoped<IConnectionProviderFactory, CustomSqlDataConnectionProviderFactory>();

            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseHsts();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseSpaStaticFiles();
            }

            LoggerService.Initialize(new ReportingLoggerService(loggerFactory.CreateLogger("DXReporting")));

            app.UseDevExpressControls();
            //app.UseStatusCodePages(async context => {
                
            //    var request = context.HttpContext.Request;
            //    var response = context.HttpContext.Response;

            //    if(response.StatusCode == (int)HttpStatusCode.Unauthorized) {
            //        // you may also check requests path to do this only for specific methods       
            //        // && request.Path.Value.StartsWith("/specificPath")
            //        response.Redirect("/Account/Login");
            //    }
            //    await Task.CompletedTask;
            //});
            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{*url}",
                //    defaults: new { controller = "Reports", action = "Index" });
                //MapSpaFallbackRoute("angular-fallback",
                //    new { controller = "Home", action = "Index" });
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
