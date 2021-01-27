using System;
using AspNetCore.Reporting.Common.Services.Reporting;
using DevExpress.AspNetCore.Reporting;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Web.QueryBuilder.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.Extensions.DependencyInjection;
using IDataSourceWizardConnectionStringsProvider = DevExpress.DataAccess.Web.IDataSourceWizardConnectionStringsProvider;

namespace AspNetCore.Reporting.Common.Services {
    public class ServiceRegistrator {
        public static IServiceCollection AddCommonServices(IServiceCollection services) {
            var cacheCleanerSettings = new CacheCleanerSettings(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
            services.AddSingleton<CacheCleanerSettings>(cacheCleanerSettings);

            var storageCleanerSettings = new StorageCleanerSettings(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(12), TimeSpan.FromHours(12), TimeSpan.FromHours(12));
            services.AddSingleton<StorageCleanerSettings>(storageCleanerSettings);

            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner((reportDesignerConfigurator) => {
                    reportDesignerConfigurator.RegisterObjectDataSourceConstructorFilterService<CustomObjectDataSourceConstructorFilterService>();
                    //reportDesignerConfigurator.RegisterObjectDataSourceWizardTypeProvider<CustomObjectDataSourceWizardTypeProvider>();
                    //reportDesignerConfigurator.RegisterDataSourceWizardConnectionStringsProvider<CustomSqlDataSourceWizardConnectionStringsProvider>(true);
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    // StorageSynchronizationMode.InterThread - it is a default value, use InterProcess if you use multiple application instances without ARR Affinity
                    viewerConfigurator.UseFileDocumentStorage("ViewerStorages\\Documents", StorageSynchronizationMode.InterThread);
                    //viewerConfigurator.UseFileExportedDocumentStorage("ViewerStorages\\ExportedDocuments", StorageSynchronizationMode.InterThread);
                    viewerConfigurator.UseFileReportStorage("ViewerStorages\\Reports", StorageSynchronizationMode.InterThread);
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            services.AddScoped<IWebDocumentViewerExceptionHandler, CustomWebDocumentViewerExceptionHandler>();
            services.AddScoped<IReportDesignerExceptionHandler, CustomReportDesignerExceptionHandler>();
            services.AddScoped<IQueryBuilderExceptionHandler, CustomQueryBuilderExceptionHandler>();

            services.AddScoped<IWebDocumentViewerAuthorizationService, DocumentViewerAuthorizationService>();
            services.AddScoped<WebDocumentViewerOperationLogger, DocumentViewerAuthorizationService>();

            //services.AddSingleton<IScopedDbContextProvider<SchoolDbContext>, ScopedDbContextProvider<SchoolDbContext>>();

            //services.AddScoped<IAuthenticatiedUserService, UserService>();
            services.AddScoped<IWebDocumentViewerReportResolver, WebDocumentViewerReportResolver>();
            services.AddScoped<IObjectDataSourceInjector, ObjectDataSourceInjector>();
            //services.AddTransient<ReportStorageWebExtension, EFCoreReportStorageWebExtension>();
            //services.AddTransient<CourseListReportRepository>();
            //services.AddTransient<MyEnrollmentsReportRepository>();
            services.AddScoped<PreviewReportCustomizationService, CustomPreviewReportCustomizationService>();

            services.AddScoped<IDataSourceWizardConnectionStringsProvider, CustomSqlDataSourceWizardConnectionStringsProvider>();
            services.AddScoped<IConnectionProviderService, CustomConnectionProviderService>();
            services.AddScoped<IConnectionProviderFactory, CustomSqlDataConnectionProviderFactory>();
            return services;
        }
    }
}
