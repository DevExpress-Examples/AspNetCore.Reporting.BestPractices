# ASP.NET Core Reporting - Best Practices

## Introduction

This **README** file contains information about best practices that you should follow when you develop a web application with DevExpress reporting controls. 

The repository also contains an example application that you can use to study the described techniques in action. This application is split into three projects:

- **ASP.NetCore.Reporting.MVC** - An ASP.Net Core MVC application.
- **ASP.NetCore.Reporting.Angular** - An ASP.Net Core application with an Angular frontend.
- **ASP.NetCore.Reporting.Common** - Implements services and business logic used by both MVC and Angular projects.

Note that the example application is designed as a showcase for for the described best practices should not be used as a template for a new application.

## Table of Contents:
- [Running the Example Application](#running-the-example-application)
  - [Configure NuGet](#configure-nuget)
  - [Install NPM Dependencies](#install-npm-dependencies)
  - [Start the Application](#start-the-application)
- [Optimize Memory Consumption](#optimize-memory-consumption)
- [Manage Database Connections](#manage-database-connections)
- [Application Security](#application-security)
  - [Prevent Cross-Site Request Forgery](#prevent-cross-site-request-forgery)
  - [Implement User Authorization](#implement-user-authorization)
- [Handle Exceptions](#handle-exceptions)
  - [Logging errors that occurred in the code of DevExpress reporting components](#logging-errors-that-occurred-in-the-code-of-devexpress-reporting-components)
  - [Using custom exception handlers](#using-custom-exception-handlers)
- [Prepare Skeleton Screen](#prepare-skeleton-screen)
- [Localize Client UI](#localize-client-ui)


## Running the Example Application 

Follow the steps below to run the example application.

### Configure NuGet

To run the example application, you need to install packages from the DevExpress NuGet feed. Refer to the following articles for information on how to obtain your DevExpress NuGet Feed URL and register the DevExpress NuGet Feed as a package source:

- [Obtain Your NuGet Feed URL](https://docs.devexpress.com/GeneralInformation/116042/installation/install-devexpress-controls-using-nuget-packages/obtain-your-nuget-feed-url)
- [Setup Visual Studio's NuGet Package Manager](https://docs.devexpress.com/GeneralInformation/116698/installation/install-devexpress-controls-using-nuget-packages/setup-visual-studios-nuget-package-manager)

### Install NPM Dependencies

- In the **ASP.NET Core MVC** project, run `npm install` in the root folder. 
- In the **Angular** project, navigate to the **ClientApp** directory and run `nmp instal`.

### Start the Application

> **Note:** Before you run the application, ensure that the version of DevExpress packages in the NuGet package manager matches the version of client libraries in the project's **package.json** file

Press **Run** button or F5 to run the example application.



## Optimize Memory Consumption

This section describes how to optimize a reporting application's memory consumption, prevent memory leaks and cluttering on the server.

> Refer to the [Document Viewer Lifecycle](https://docs.devexpress.com/XtraReports/401587/web-reporting/general-information/document-viewer-lifecycle) for information oh how the Document Viewer stores report data and how this data is affected by the lifecycle stage.

To optimize memory consumption, use the following techniques:

- Configure the Document Viewer to to store server data on disk instead of memory. This significantly reduces the memory consumption at the cost of performance. 

  ```cs
  configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
    // StorageSynchronizationMode.InterThread - it is a default value, use InterProcess if you use multiple application instances without ARR Affinity
    viewerConfigurator.UseFileDocumentStorage("ViewerStorages\\Documents", StorageSynchronizationMode.InterThread);
    viewerConfigurator.UseFileExportedDocumentStorage("ViewerStorages\\ExportedDocuments", StorageSynchronizationMode.InterThread);
    viewerConfigurator.UseFileReportStorage("ViewerStorages\\Reports", StorageSynchronizationMode.InterThread);
    viewerConfigurator.UseCachedReportSourceBuilder();
  });
  ```

- When the page or a UI region (for example, a popup window) that displays the Document Viewer is about to be closed, close the the viewed report to release the server resources (the Storage space and Cache). To do that use the client-side `ASPxClientWebDocumentViewer.Close` method:

  ```cs
  function WebDocumentViewer_BeforeRender(s, e) {
    $(window).on('beforeunload', function(e) {
      s.Close();
  });
  ```

- Configure Storage and Cache cleaners on application startup. This allows you to control how long document data persists on the server, and consequently, how long the server resources are reserved by it. Note that after the Storage and Cache are cleared, you cannot continue viewing (scrolling, printing, etc.) the document without re-creating it, so make sure to use reasonable values for these settings.

  ```cs
  var cacheCleanerSettings = new CacheCleanerSettings(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
  services.AddSingleton<CacheCleanerSettings>(cacheCleanerSettings);

  var storageCleanerSettings = new StorageCleanerSettings(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(12), TimeSpan.FromHours(12), TimeSpan.FromHours(12));
  services.AddSingleton<StorageCleanerSettings>(storageCleanerSettings);
  ```

  > Keep in mind that .NET is a managed environment and unused memory is freed only during garbage collection. Refer to the [Fundamentals of garbage collection](https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals) article for more information.

## Manage Database Connections

It is a good practice to serialize only connection names and implement a connection provider that returns data connections based on a name. This abstracts away all data connection logics and hides all connection parameters from the client.

Reporting services retrieve the IConnectionProviderFactory and IDataSourceWizardConnectionStringsProvide through Dependency Injection. To learn how to implement these services, refer to the following example project's files:

- [Services/Reporting/CustomSqlDataConnectionProviderFactory.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomSqlDataConnectionProviderFactory.cs)
- [Services/Reporting/CustomSqlDataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomSqlDataSourceWizardConnectionStringsProvider.cs)

To ensure that encrypted connection parameters for SqlDataSource instances are not passed to the client,
return null in the `IDataSourceWizardConnectionStringsProvider.GetDataConnectionParameters` method implementation:

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

Register the implemented services in [Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs)




## Application Security

### Prevent Cross-Site Request Forgery

An antiforgery token is generate when the client-side reporting application is rendered on a view. The token is saved on the client and the client application passes it to the server with every request for verification.

For more information on how to use antiforgery tokens in ASP.NET, refer to the following resources: 

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

To perform user authorization and restrict access to reports based on arbitrary logic, implement and register an `IWebDocumentViewerAuthorizationService` along with `WebDocumentViewerOperationLogger`.

Additionally, implement an `IWebDocumentViewerExportedDocumentStorage` to prevent unauthorized requests to documents generated during asynchronous export and printing operations.

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

    // The code below defines authorization rules applyed to different operations on reports.
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

This document section describes the best practices that you should follow when you handle and log errors in a reporting application. For information on how to determine the exact cause of a problem with your application, refer to the [Reporting Application Diagnostics](https://docs.devexpress.com/XtraReports/401687/web-reporting/general-information/application-diagnostics) documentation topic.

### Logging errors that occurred in the code of DevExpress reporting components

To handle exceptions generated by DevExpress reporting components, implement and register a custom logger service. In your implementation of the service, override the `Error` method. In the `Error` method's implementation, save errors to a log or database. If a Visual Studio debugger is attached, you can set a breakpoint and inspect errors in the Watch window.

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

### Using custom exception handlers

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

This section describes how to implement a skeleton screen to maximize the application's responsiveness. With this approach, the client first loads a mock screen that mimics the application's layout and then proceeds to load the resources for the reporting components.

![web-skeleton-designer](https://user-images.githubusercontent.com/37070809/94427328-8f40a500-0197-11eb-87c4-82ec9862b148.png)

Use the following steps to prepare a skeleton.

In [\_Layout.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Shared/_Layout.cshtml), move all script registrations to the bottom of the layout:

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

In a view, render two separate parts of the reporting control:

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


## Localize Client UI

DevExpress client reporting controls use the DevExtreme localization mechanism to localize the UI and messages.

To localize DevExpress reporting controls, go to [localization.devexpress.com](localization.devexpress.com), download the required localization JSON files and save them to the `locaization` folder within the application's wwwroot folder. After that, configure the view as described below:

1. On the client `CustomizeLocalization` event, load the localization JSON files:

```
    function CustomizeLocalization(s, e){
        e.LoadMessages($.get("/localization/reporting/dx-analytics-core.de.json"));
        e.LoadMessages($.get("/localization/reporting/dx-reporting.de.json"));
        e.LoadMessages($.get("/localization/devextreme/de.json").done(function(messages) { DevExpress.localization.loadMessages(messages); }));
        e.SetAvailableCultures(["de"]);
    }
```

2. Set the `IncludeLocalization` option to `false` to disable automatic attachment of the localization dictionary:

```cs
    Html.DevExpress().ReportDesigner("ReportDesigner").
        .ClientSideModelSettings(clientSide => {
            clientSide.IncludeLocalization = false;
        })
        ...
```

The translation is not guaranteed to contain all strings required by your application. You can use the localization service's UI to add the required strings. See the [Localization Service](https://docs.devexpress.com/LocalizationService/16235/localization-service) article for more information.

For a full code example, refer to the example project's [Views/Home/DesignReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml) or [Views/Home/DisplayReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DisplayReport.cshtml).


----------------------


## Intro

Topics importance levels based on subject relevance in SC tickets:
* High
* Common
* Rare

## Authorization (High)

For all API calls except print and export operations, the Web Document Viewer and Report Designer components send AJAX requests to the server. To apply a custom authorization header to these requests it is necessary to use the `DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings` property:

```javascript=
DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = {  
    headers: { 'Authorization': token }  
}; 
```

### Printing and Export specifics
For print and export operations, a web browser requests the result from the server, but these requests have no headers. On the server side, the WebDocumentViewerApiController's Invoke action processes all requests. When your application uses header-token authentication, this controller action is protected (for instance, with a Bearer authentication). A web browser cannot obtain export and print results, because its requests have no headers and cannot be authenticated. 

To overcome this issue use the ```IWebDocumentViewerExportResultUriGenerator``` service: Implement the IWebDocumentViewerExportResultUriGenerator interface and register it in the service container. In the CreateUri method, save an exported document to any storage and return the URI to access it from the client side.

Example (NOT ASP.NET CORE) https://github.com/DevExpress-Examples/Reporting_How-to-export-documents-with-token-based-authentication-in-the-web-document-viewer

To make this approach it is necessary to enable the asynchronous export mechanism on the Web Document Viewer's client-side by using the `DevExpress.Report.Preview.AsyncExportApproach` property:
```javascript=
DevExpress.Report.Preview.AsyncExportApproach = true;
```

>Note: 90% of questions regarding the Authorization are related to Angular applications that use ASP.NET Core as a backed. So, the client-side code should demonstrate Angular code snippets as well as JS ones

Text Source:
https://github.com/DevExpress-Examples/Reporting_How-to-export-documents-with-token-based-authentication-in-the-web-document-viewer


## Antiforgery (Low)

Our components use jQuery.Ajax function to send requests to the web server. So, you can generate a token and pass it in the request header as it's explained in this MSDN help  topic:  [Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery). For example use the following code for this purpose (CODE NEEDS VERIFICATION):
```razor=
@inject Microsoft.AspNetCore.Antiforgery.IAntiforgery Xsrf  
@functions{  
    public string GetAntiXsrfRequestToken() {  
        return Xsrf.GetAndStoreTokens(this.Context).RequestToken;  
    }  
}  
<script>  
    function customizeMenu() {  
        DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = { headers: { 'RequestVerificationToken': document.getElementById('RequestVerificationToken').value } };  
    }  
</script>  
<input type="hidden" id="RequestVerificationToken" name="RequestVerificationToken" value="@GetAntiXsrfRequestToken()">  
@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").ClientSideEvents((a) => a.CustomizeMenuActions("customizeMenu")).Bind("Report")  
```
The key point here is to use the `DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings` property to add your custom token to all requests that are sent by using jQuery.Ajax function from our Reporting widgets.

Text Source:
https://supportcenter.devexpress.com/internal/ticket/details/T871976

## Memory Consumption (High)

To reduce the memory consumption it is necessary to do the following:

1. Use the [CachedReportSourceWeb](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.Web.CachedReportSourceWeb) or [ICachedReportSourceWebResolver](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.Web.WebDocumentViewer.ICachedReportSourceWebResolver) to supply viewer with a report
2. Use the [UseFileDocumentStorage](https://docs.devexpress.com/XtraReports/DevExpress.AspNetCore.Reporting.WebDocumentViewerConfigurationBuilder.UseFileDocumentStorage(System.String)) method to enable file cache (DESCRIBE OTHER CACHES HERE? DB/AZURE)
3. Force viewer to close the document when browser's window/tab is closed and release resources consumed by this document. To do that use the client-side ```ASPxClientWebDocumentViewer.Close``` method (CODE NEEDS VERIFICATION):
```javascript=
    function WebDocumentViewer_BeforeRender(s, e) {  
        window.onbeforeunload = function (evt) {  
            s.Close();  
        }  
    }  
```
4. Adjust viewer's caching and storage settings by using the  `CacheCleanerSettings` and `StorageCleanerSettings` settings. Check the [Document Viewer Lifecycle](https://docs.devexpress.com/XtraReports/401587/create-end-user-reporting-applications/web-reporting/general-information/document-viewer-lifecycle) help topic for more information regarding the viewer's internal architecture.

>NOTE: According to the .NET memory managing specifics, the unused memory is cleared only after running the garbage collection. Check the [Fundamentals of garbage collection](https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals) help topic for more information regarding this process.

### Export Notes

Currently, only our PDF export engine supports so-called "streaming" mode. So, to export a report to any other format (e.g. Excel), it is necessary to store the whole document in the memory. This is the reason why the whole document is loaded from the chache when export to non-PDF format is triggered.


## Deployment (Common)

Do not forget to deploy all JS resources and Reports storage folder (if a filesystem storage is used) to the web server along with the project. To do that you can use the **Publish** option in Visual Studio:
![](https://hackmd.devexpress.devx/uploads/upload_b80272ca9bf975e8c6fa0ac255cd5628.png)


Source: https://supportcenter.devexpress.com/internal/ticket/details/T801637

### Web Farm

[Web Farms and Web Garden Support](https://docs.devexpress.com/XtraReports/5199/create-end-user-reporting-applications/web-reporting/general-information/web-farms-and-web-gardens-support)

### Azure

[Microsoft Azure Reporting](https://docs.devexpress.com/XtraReports/10769/create-end-user-reporting-applications/web-reporting/general-information/microsoft-azure-reporting)


## Exception Handling (Common)

### Logging Errors in code processed by Reporting Componens

Register a custom `LoggerService`. For this service, implement an `Error` method. In the method’s implementation, save the errors to a log file or database. If a debugger is attached, you can also set a breakpoint and view the incoming errors in the Watch window. 

### Using custom exception handlers 

This approach demonstrates how to show custom exception messages instead of default ones:
[Handle Server-Side Errors in the Document Viewer](https://docs.devexpress.com/XtraReports/400524/create-end-user-reporting-applications/web-reporting/asp-net-core-reporting/document-viewer/api-and-customization/handle-server-side-errors-in-the-document-viewer)
[Handle Server-Side Errors in the Report Designer](https://docs.devexpress.com/XtraReports/400525/create-end-user-reporting-applications/web-reporting/asp-net-core-reporting/end-user-report-designer/api-and-customization/handle-server-side-errors-in-the-report-designer)

### Troubleshooting

[Troubleshooting](https://docs.devexpress.com/XtraReports/401726/create-end-user-reporting-applications/web-reporting/general-information/troubleshooting)


## Reports/Documents/HTTPHandlers Availability (Rare)

404 errors? 

## To Release used memory send Close request on beforeUnload: viewer.Close() issues with routes of ASP.NET Core reporting controllers

Already covered above.


## IWebDocumentViewerAuthorizationService (?)

What should be here?


## Skeleton while scripts loading (+defer render, +bundling, +prevent caching) (N/A)

New feature, I am not aware of details


## Connection providers (serialize connection name only, prod/dev environment)

What should be here?

## How to render viewer on page to make it responsive (height 100%)

Maybe this should be joined with Skeleton and describe common layout configuration details?

You need to adjust your web page's layout to make the WebDocumentViewer control's height responsive - it should be adjusted to the browser's screen height. All parent elements' height should be also set to full screen. For example:
```css
html, body {
  height: 100%;
}
```


Source: https://www.w3schools.com/howto/howto_css_div_full_height.asp

### Responsive / Mobile UI

We do not support Responsive UI. But the WebDocumentViewer supports [Mobile Mode](https://docs.devexpress.com/XtraReports/17738/create-end-user-reporting-applications/web-reporting/asp-net-webforms-reporting/document-viewer) (NO DOC FOR ASP.NET CORE, WHAT A SHAME)

The mobile mode should be enabked manually, depending on the User-Agent (VERIFICATION IS REQUIRED)

## Localization (High)

### UI

1. DevExtreme - [Localization](https://js.devexpress.com/Documentation/Guide/Common/Localization/)
2. Localization Service
3. [Localize the User Interface](https://docs.devexpress.com/XtraReports/400932/create-end-user-reporting-applications/web-reporting/asp-net-core-reporting/localization#localize-the-user-interface)

### Report

[Localize Reports](https://docs.devexpress.com/XtraReports/400932/create-end-user-reporting-applications/web-reporting/asp-net-core-reporting/localization#localize-reports)


## Report Storage Recommendations (Common)

Do not use the default filesystem report storage implememntation from our project template, its bad, mmm kay? (It is for demonstration purposes only)
