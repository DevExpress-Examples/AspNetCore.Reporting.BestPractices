# ASP.NET Core Reporting - Best Practices

## Introduction

This **README** file describes best practices to follow when you develop a web application with DevExpress reporting controls.

This repository also contains an example application that demonstrates the described techniques. This application is split into three projects:

- **ASP.NetCore.Reporting.MVC** - An ASP.Net Core MVC application.
- **ASP.NetCore.Reporting.Angular** - An ASP.Net Core application with an Angular frontend.
- **ASP.NetCore.Reporting.Common** - Implements services and business logic for the MVC and Angular projects.

You can use the example code in your web application and modify it for different scenarios.

## Table of Contents:

- [How to run the Example Application](#how-to-run-the-example-application)
  - [Configure NuGet](#configure-nuget)
  - [Install NPM Dependencies](#install-npm-dependencies)
  - [Start the Application](#start-the-application)
- [Optimize Memory Consumption](#optimize-memory-consumption)
- [Manage Database Connections](#manage-database-connections)
- [Application Security](#application-security)
  - [Prevent Cross-Site Request Forgery](#prevent-cross-site-request-forgery)
  - [Implement User Authorization](#implement-user-authorization)
- [Handle Exceptions](#handle-exceptions)
  - [Log errors that occurred in the code of DevExpress reporting components](#log-errors-that-occurred-in-the-code-of-devexpress-reporting-components)
  - [Use custom exception handlers](#use-custom-exception-handlers)
- [Prepare Skeleton Screen](#prepare-skeleton-screen)
- [Localize Client UI](#localize-client-ui)

## How to run the Example Application

Follow the steps below to run the example application in Microsoft Visual Studio.

### Configure NuGet

To run the example application, you need to install packages from the DevExpress NuGet feed. Use the following steps to configure NuGet:

1. [Obtain Your NuGet Feed URL](https://docs.devexpress.com/GeneralInformation/116042/installation/install-devexpress-controls-using-nuget-packages/obtain-your-nuget-feed-url)
2. [Register the NuGet feed as a package sources](https://docs.devexpress.com/GeneralInformation/116698/installation/install-devexpress-controls-using-nuget-packages/setup-visual-studios-nuget-package-manager)

### Install NPM Dependencies

- For the **ASP.NET Core MVC** project, run `npm install` in the project's root folder.
- For the **Angular** project, navigate to the **ClientApp** directory and run `npm install`.

### Start the Application

> **Note:** If you change the version of DevExpress NuGet packages used in the example application, make sure you also specify the matching minor versions for DevExpress client libraries in the **package.json** file.

Press the **Run** button or F5 to run the example application.

## Optimize Memory Consumption

This section describes how to optimize a reporting application's memory consumption, and prevent memory leaks and cluttering on the server.

> Refer to the [Document Viewer Lifecycle](https://docs.devexpress.com/XtraReports/401587/web-reporting/general-information/document-viewer-lifecycle) for information oh how the Document Viewer stores report data on different lifecycle stages.

To optimize memory consumption, use the following techniques:

- Configure the Document Viewer to to store server data on disk instead of memory. This reduces the memory consumption at the cost of performance.

  ```cs
  configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
    // StorageSynchronizationMode.InterThread - it is a default value, use InterProcess if you use multiple application instances without ARR Affinity
    viewerConfigurator.UseFileDocumentStorage("ViewerStorages\\Documents", StorageSynchronizationMode.InterThread);
    viewerConfigurator.UseFileExportedDocumentStorage("ViewerStorages\\ExportedDocuments", StorageSynchronizationMode.InterThread);
    viewerConfigurator.UseFileReportStorage("ViewerStorages\\Reports", StorageSynchronizationMode.InterThread);
    viewerConfigurator.UseCachedReportSourceBuilder();
  });
  ```

- When a page or a UI region (for example, a pop-up window) that displays the Document Viewer is about to be closed, close the the viewed report to release the server resources (the Storage space and Cache). To do that use the Document Viewer's client-side `Close` method:

  ```cs
  function WebDocumentViewer_BeforeRender(s, e) {
    $(window).on('beforeunload', function(e) {
      s.Close();
  });
  ```

- Configure Storage and Cache cleaners on application startup. This allows you to specify how long document data persists on the server, and consequently, how long the server resources are reserved to store this data. Note that after a document's data is removed for the Storage and Cache, you cannot navigate or print this document - make sure to use reasonable values for these settings.

  ```cs
  var cacheCleanerSettings = new CacheCleanerSettings(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
  services.AddSingleton<CacheCleanerSettings>(cacheCleanerSettings);

  var storageCleanerSettings = new StorageCleanerSettings(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(12), TimeSpan.FromHours(12), TimeSpan.FromHours(12));
  services.AddSingleton<StorageCleanerSettings>(storageCleanerSettings);
  ```

  > Keep in mind that .NET is a managed environment, so data saved to the disk storage and removed from cache remains in memory until the garbage collection runs. Refer to the [Fundamentals of garbage collection](https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals) article for more information.

## Manage Database Connections

DevExpress reporting components are configured to retrieve database connections from the application configuration file. This mechanism is secure: a serialized report contains only the connection name. If you implement a custom connection provider to customize this mechanism (for example, to filter the list of connections), ensure you serialize only the data connection's name and do not pass connection parameters to the client.

Reporting services obtain an IConnectionProviderFactory and IDataSourceWizardConnectionStringsProvide through Dependency Injection. For instructions on how to implement these services, refer to the following example project's files:

- [Services/Reporting/CustomSqlDataConnectionProviderFactory.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomSqlDataConnectionProviderFactory.cs)
- [Services/Reporting/CustomSqlDataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomSqlDataSourceWizardConnectionStringsProvider.cs)

To ensure that encrypted connection parameters for SqlDataSource instances are not passed to the client, return `null` from the `IDataSourceWizardConnectionStringsProvider.GetDataConnectionParameters` method's implementation:

```cs
public DataConnectionParametersBase GetDataConnectionParameters(string name) {
  return null;
}
```

In the IConnectionProviderService returned by the IConnectionProviderFactory, initialize and return the connection.</span>

```cs
public SqlDataConnection LoadConnection(string connectionName) {
    var connectionStringSection = configuration.GetSection("ReportingDataConnectionStrings");
    var connectionString = connectionStringSection?.GetValue<string>(connectionName);
    var connectionStringInfo = new ConnectionStringInfo { RunTimeConnectionString = connectionString, ProviderName = "SQLite" };
    DataConnectionParametersBase connectionParameters;
    if(string.IsNullOrEmpty(connectionString)
        || !AppConfigHelper.TryCreateSqlConnectionParameters(connectionStringInfo, out connectionParameters)
        || connectionParameters == null) {
        throw new KeyNotFoundException($"Connection string '{connectionName}' not found.");
    }
    return new SqlDataConnection(connectionName, connectionParameters);
}
```

Register the implemented services in [Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs).

## Application Security

### Prevent Cross-Site Request Forgery

To prevent cross-site request forgery, DevExpress reporting controls use the standard ASP.NET Core mechanism described in the following articles:

##### Microsoft Documentation:

- [Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery).

##### DevExpress Security Best Practices:

- [ASP.NET WebForms - Preventing Cross-Site Request Forgery (CSRF)](https://github.com/DevExpress/aspnet-security-bestpractices/tree/master/SecurityBestPractices.WebForms#4-preventing-cross-site-request-forgery-csrf)
- [ASP.NET MVC - Preventing Cross-Site Request Forgery (CSRF)](https://github.com/DevExpress/aspnet-security-bestpractices/tree/master/SecurityBestPractices.Mvc#4-preventing-cross-site-request-forgery-csrf)

The following code samples demonstrate how to apply antiforgery request validation on the Document Viewer's and Report Designer's controller actions.

##### Document Viewer

```cs
[ValidateAntiForgeryToken]
public class CustomMVCWebDocumentViewerController : WebDocumentViewerController {
    public CustomMVCWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService) {
    }

    public override Task<IActionResult> Invoke() {
        return base.Invoke();
    }
}
```

##### Report Designer

```cs
[ValidateAntiForgeryToken]
public class CustomMVCQueryBuilderController : QueryBuilderController {
    public CustomMVCQueryBuilderController(IQueryBuilderMvcControllerService controllerService) : base(controllerService) {
    }
    public override Task<IActionResult> Invoke() {
         return base.Invoke();
    }
}

[ValidateAntiForgeryToken]
public class CustomMVCReportDesignerController : ReportDesignerController {
    public CustomMVCReportDesignerController(IReportDesignerMvcControllerService controllerService) : base(controllerService) {
    }

    public override Task<IActionResult> Invoke() {
        return base.Invoke();
    }
}
```

See the example project's [Views/Home/DesignReport](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml) or [Views/Home/DisplayReport](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DisplayReport.cshtml) file for the full code.

### Implement User Authorization

To authorize a user and restrict access to reports based on arbitrary logic, implement and register an `IWebDocumentViewerAuthorizationService` with `WebDocumentViewerOperationLogger`.

You can also implement an `IWebDocumentViewerExportedDocumentStorage` to prevent unauthorized access to documents generated during asynchronous export and printing operations.

[Services/Reporting/DocumentViewerAuthorizationService.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/DocumentViewerAuthorizationService.cs):

```cs
class DocumentViewerAuthorizationService : WebDocumentViewerOperationLogger, IWebDocumentViewerAuthorizationService {
    static ConcurrentDictionary<string, string> DocumentIdOwnerMap { get; } = new ConcurrentDictionary<string, string>();
    static ConcurrentDictionary<string, string> ReportIdOwnerMap { get; } = new ConcurrentDictionary<string, string>();

    IAuthenticatiedUserService UserService { get; }

    public DocumentViewerAuthorizationService(IAuthenticatiedUserService userService) {
        UserService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    // The code below overrides the WebDocumentViewerOperationLogger's methods to intersect report
    // and document creation operations and associats the report and document IDs with the owner's ID
    public override void ReportOpening(string reportId, string documentId, XtraReport report) {
        MapIdentifiersToUser(UserService.GetCurrentUserId(), documentId, reportId);
        base.ReportOpening(reportId, documentId, report);
    }

    public override void BuildStarted(string reportId, string documentId, ReportBuildProperties buildProperties) {
        MapIdentifiersToUser(UserService.GetCurrentUserId(), documentId, reportId);
        base.BuildStarted(reportId, documentId, buildProperties);
    }

    void MapIdentifiersToUser(string userId, string documentId, string reportId) {
        if(!string.IsNullOrEmpty(documentId)) {
            DocumentIdOwnerMap.TryAdd(documentId, userId);
        }
        if(!string.IsNullOrEmpty(reportId)) {
            ReportIdOwnerMap.TryAdd(reportId, userId);
        }
    }

    // The code below defines authorization rules applied to different operations on reports.
    #region IWebDocumentViewerAuthorizationService
    public bool CanCreateDocument() {
        return true;
    }

    public bool CanCreateReport() {
        return true;
    }

    public bool CanReadDocument(string documentId) {
        return DocumentIdOwnerMap.TryGetValue(documentId, out var ownerId) && ownerId == UserService.GetCurrentUserId();
    }

    public bool CanReadReport(string reportId) {
        return ReportIdOwnerMap.TryGetValue(reportId, out var ownerId) && ownerId == UserService.GetCurrentUserId();
    }

    public bool CanReleaseDocument(string documentId) {
        return true;
    }

    public bool CanReleaseReport(string reportId) {
        return true;
    }
    #endregion
}
```

Register your authorization service implementation in **startup.cs**.

[Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs#L106):

```cs
public class Startup {
    public void ConfigureServices(IServiceCollection services) {
        ...
        services.AddScoped<IWebDocumentViewerAuthorizationService, DocumentViewerAuthorizationService>();
        ...
    }
    ...
}
```

## Handle Exceptions

This document section describes the best practices that you should follow when you handle and log errors in a reporting application. For information on how to determine the cause of a problem with your application, refer to the [Reporting Application Diagnostics](https://docs.devexpress.com/XtraReports/401687/web-reporting/general-information/application-diagnostics) documentation topic.

### Log errors that occurred in the code of DevExpress reporting components

To handle exceptions generated by DevExpress reporting components, implement and register a logger service and override the `Error` method in your service implementation. In the `Error` method's implementation, save errors to a log or database. If a Visual Studio debugger is attached, you can set a breakpoint and inspect errors in the Watch window.

##### Implement a logger class

[Services/Reporting/ReportingLoggerService.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/ReportingLoggerService.cs):

```cs
public class ReportingLoggerService: LoggerService {
    readonly ILogger logger;
    public ReportingLoggerService(ILogger logger) {
        this.logger = logger;
    }
    public override void Error(Exception exception, string message) {
        var logMessage = $"[{DateTime.Now}]: Exception occurred. Message: '{message}'. Exception Details:\r\n{exception}";
        logger.LogError(logMessage);
    }

    public override void Info(string message) {
        logger.LogInformation(message);
    }
}
```

##### Register the logger in **startup.cs**

[Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs#L139):

```cs
LoggerService.Initialize(new ReportingLoggerService(loggerFactory.CreateLogger("DXReporting")));
```

### Use custom exception handlers

Use custom exception handler services to customize error details that are passed to the client and displayed in the client UI:

```cs


public class CustomWebDocumentViewerExceptionHandler : WebDocumentViewerExceptionHandler {
    public override string GetExceptionMessage(Exception ex) {
        if(ex is FileNotFoundException) {
        #if DEBUG
            return ex.Message;
        #else
            return "File is not found.";
        #endif
        }
        return base.GetExceptionMessage(ex);
    }
    public override string GetUnknownExceptionMessage(Exception ex) {
        return $"{ex.GetType().Name} occurred. See the log file for more details.";
    }
}
```

Refer to the example project's [Services/Reporting/CustomExceptionHandlers.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomExceptionHandlers.cs) file for the full code.

## Prepare Skeleton Screen

This section describes how to implement a skeleton screen that indicates that the application is being loaded. With this approach, the client first displays a mock screen that mimics the application's layout and then proceeds to load the resources for the reporting components.

![web-skeleton-designer](https://user-images.githubusercontent.com/37070809/94427328-8f40a500-0197-11eb-87c4-82ec9862b148.png)

Use the following steps to prepare a skeleton:

In [\_Layout.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Shared/_Layout.cshtml), move all the `script` elements to the bottom of the layout:

```cs
...
    <footer class="border-top footer text-muted">
        <div class="container">
            <p>&copy; @DateTime.Now.Year - ASP.NET Core Reporting Demo Application</p>
        </div>
    </footer>
    <script src="~/js/site.thirdparty.bundle.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @RenderSection("Scripts", false)
</body>
</html>
```

In a view, add the `dx-reporting-skeleton-screen.css` file from the **devexpress-reporting** NPM package to the page that contains the Report Designer and render two separate parts of the reporting control:

- Call the `RenderHtml()` method to render markup.
- Call the `RenderScripts()` method to render scripts.

[Views/Home/DesignReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml):

```cs
@{
    var designerRender = Html.DevExpress().ReportDesigner("ReportDesigner").Height("800px").Bind(Model.Id);//you can set another options here
    @:@designerRender.GetHtml()
}

...

@section Scripts {
    <link href="~/css/dx-reporting-skeleton-screen.css" rel="stylesheet" />
    <link rel="stylesheet" href="~/css/viewer.part.bundle.css" />
    <link rel="stylesheet" href="~/css/designer.part.bundle.css" />

    <script src="~/js/reporting.thirdparty.bundle.js"></script>
    <script src="~/js/viewer.part.bundle.js"></script>
    <script src="~/js/designer.part.bundle.js"></script>
    @designerRender.RenderScripts()
}
```

Refer to the [Enable the Skeleton Screen](https://docs.devexpress.com/XtraReports/401830/web-reporting/asp-net-core-reporting/end-user-report-designer/customization/enable-skeleton-screen) article for more information.

## Localize Client UI

DevExpress client reporting controls use the DevExtreme localization mechanism to localize the UI and messages.

To localize DevExpress reporting controls, go to [localization.devexpress.com](localization.devexpress.com), download the JSON files for the localization, and save them to the `locaization` folder within the application's `wwwroot` folder. After that, configure the view as described below:

1. In the client `CustomizeLocalization` event, load the localization JSON files:

```js
function CustomizeLocalization(s, e) {
  e.LoadMessages($.get("/localization/reporting/dx-analytics-core.de.json"));
  e.LoadMessages($.get("/localization/reporting/dx-reporting.de.json"));
  e.LoadMessages(
    $.get("/localization/devextreme/de.json").done(function (messages) {
      DevExpress.localization.loadMessages(messages);
    })
  );
  e.SetAvailableCultures(["de"]);
}
```

2. Set the `IncludeLocalization` option to `false` to disable automatic attachment of the localization dictionary:

```cs
    Html.DevExpress().ReportDesigner("ReportDesigner")
        .ClientSideModelSettings(clientSide => {
            clientSide.IncludeLocalization = false;
        })
        ...
```

A translation does not always cover all the possible strings. You can use the localization service's UI to add the missing strings as described in the [Localization Service](https://docs.devexpress.com/LocalizationService/16235/localization-service) article.

For a full code example, refer to the example project's [Views/Home/DesignReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml) or [Views/Home/DisplayReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DisplayReport.cshtml).
