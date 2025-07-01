using System;
using AspNetCore.Reporting.Angular.Data;
using AspNetCore.Reporting.Angular.Server.Services;
using AspNetCore.Reporting.Common.Data;
using AspNetCore.Reporting.Common.Reports;
using AspNetCore.Reporting.Common.Services;
using AspNetCore.Reporting.Common.Services.Reporting;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.Security.Resources;
using DevExpress.Utils;
using DevExpress.XtraReports.Web.Extensions;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

AppDomain.CurrentDomain.SetData("DataDirectory", builder.Environment.ContentRootPath);
builder.Services.AddDevExpressControls();
builder.Services.AddDbContext<SchoolDbContext>(options =>
    //.UseSqlServer(builder.Configuration.GetConnectionString("DefaultMSSqlConnection")));
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultSqliteConnection")));

builder.Services.AddDefaultIdentity<StudentIdentity>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddUserManager<UserManager<StudentIdentity>>()
    .AddEntityFrameworkStores<SchoolDbContext>();
builder.Services.AddIdentityServer()
.AddApiAuthorization<StudentIdentity, SchoolDbContext>(options => {
    var api = options.ApiResources.FirstOrDefault();
    api.UserClaims = new[] { System.Security.Claims.ClaimTypes.Sid };
});

builder.Services.AddAuthentication().AddIdentityServerJwt();
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontendDev", policy => {
        policy.WithOrigins("https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.ConfigureReportingServices(x => {
    if(builder.Environment.IsDevelopment()) {
        x.UseDevelopmentMode();
    }
    x.ConfigureReportDesigner(reportDesignerConfigurator => {
        reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<CustomObjectDataSourceWizardTypeProvider>();
    });
});
ServiceRegistrator.AddCommonServices(builder.Services, builder.Environment.ContentRootPath);

builder.Services.AddSingleton<IScopedDbContextProvider<SchoolDbContext>, ScopedDbContextProvider<SchoolDbContext>>();
builder.Services.AddScoped<IAuthenticatiedUserService, UserService<SchoolDbContext>>();
builder.Services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension<SchoolDbContext>>();
builder.Services.AddTransient<CourseListReportRepository>();
DeserializationSettings.RegisterTrustedClass(typeof(CourseListReportRepository));
builder.Services.AddTransient<MyEnrollmentsReportRepository>();
DeserializationSettings.RegisterTrustedClass(typeof(MyEnrollmentsReportRepository));

var app = builder.Build();
using(var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<SchoolDbContext>();
        var userManager = services.GetRequiredService<UserManager<StudentIdentity>>();
        DbInitializer.Initialize(context, userManager, new ReportsFactory());
    } catch(Exception exception) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An error occurred while seeding the database.");
    }
}
app.UseDevExpressControls();
if(app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//var contentDirectoryAllowRule = DirectoryAccessRule.Allow(new DirectoryInfo(Path.Combine(app.Environment.ContentRootPath, "Content")).FullName);
//AccessSettings.ReportingSpecificResources.SetRules(contentDirectoryAllowRule, UrlAccessRule.Allow());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontendDev");
app.UseRouting();
app.UseDevExpressControls();
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("/index.html");

app.Run();