## Application Security

### Prevent Cross-Site Request Forgery

An antiforgery token is generate when the client-side reporting application is rendered on a view. The token is saved on the client and the client application passes it to the server with every request for verification.

For more information on how to use antiforgery tokens in ASP.NET, refer to the following resources: 

##### Microsoft Documentation:

- [Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery).

##### DevExpress Security Best Practices:

- [ASP.NET WebForms - Preventing Cross-Site Request Forgery (CSRF)](https://github.com/DevExpress/aspnet-security-bestpractices/tree/master/SecurityBestPractices.WebForms#4-preventing-cross-site-request-forgery-csrf)
- [ASP.NET MVC - Preventing Cross-Site Request Forgery (CSRF)](https://github.com/DevExpress/aspnet-security-bestpractices/tree/master/SecurityBestPractices.Mvc#4-preventing-cross-site-request-forgery-csrf)


The following code samples demonstrate how to apply antyforgery request validation on the Document Viewer's and Report Designer's controller actions.

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

## Optimize Memory Consumption

This section describes how to optimize a reporting application's memory consumption, prevent memory leaks and cluttering on the server.

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

![Skeleton Screen](https://docs.devexpress.com/XtraReports/images/web-skeleton-designer.gif)

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
    @designerRender.GetScripts()
}
```

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

## How to Bind a Report to an EF Core Data Source

You can bind a DevExpress report to an Entity Framework Core data source. For information on how to do this, refer to the following example:

https://github.com/DevExpress-Examples/Reporting-Entity-Framework-Core-In-AspNet-Core

---


## Antiforgery (Low)

На сервере в контроллерах вешается аттрибут [ValidateAntiForgeryToken]

[Controllers/CustomMVCReportingControllers](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Controllers/CustomMVCReportingControllers.cs)

Перед рендером клиентской части MVC генерируем этот токен во Views и сохраняем его на клиенте для аттача в каждом запросе к репорт контроллерам:

[Views/Home/DesignReport](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml)

[Views/Home/DisplayReport](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DisplayReport.cshtml)

```cs
@inject Microsoft.AspNetCore.Antiforgery.IAntiforgery Xsrf
@functions{
    public string GetAntiXsrfRequestToken() {
        return Xsrf.GetAndStoreTokens(this.Context).RequestToken;
    }
}
```

```
SetupJwt('bearer token can be passed here', "@GetAntiXsrfRequestToken()");
```

сама функция SetupJwt общая и реализована в [wwwroot/site.js](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/wwwroot/js/site.js#L1)

## Memory Consumptions

1. UseCachedReportSourceBuilder() + UseFileXXXStorage

```js
configurator.ConfigureWebDocumentViewer((viewerConfigurator) => {
  // StorageSynchronizationMode.InterThread - it is a default value, use InterProcess if you use multiple application instances without ARR Affinity
  viewerConfigurator.UseFileDocumentStorage(
    "ViewerStorages\\Documents",
    StorageSynchronizationMode.InterThread
  );
  viewerConfigurator.UseFileExportedDocumentStorage(
    "ViewerStorages\\ExportedDocuments",
    StorageSynchronizationMode.InterThread
  );
  viewerConfigurator.UseFileReportStorage(
    "ViewerStorages\\Reports",
    StorageSynchronizationMode.InterThread
  );
  viewerConfigurator.UseCachedReportSourceBuilder();
});
```

2. Close on unload
   [Views/Home/DisplayReport](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DisplayReport.cshtml)

```
    function WebDocumentViewer_BeforeRender(s, e) {
        $(window).on('beforeunload', function(e) {
            s.Close();
        });
```

3. Adjust Storage and Cache cleaners settings:
   [Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs#L80)

```
            var cacheCleanerSettings = new CacheCleanerSettings(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
            services.AddSingleton<CacheCleanerSettings>(cacheCleanerSettings);

            var storageCleanerSettings = new StorageCleanerSettings(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(12), TimeSpan.FromHours(12), TimeSpan.FromHours(12));
            services.AddSingleton<StorageCleanerSettings>(storageCleanerSettings);
```

## Custom Exception Handlers

### Logging Errors in code processed by Reporting Componens

Custom logger class:

[Services/Reporting/ReportingLoggerService.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/ReportingLoggerService.cs)

Registration in startup.cs

[Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs#L139)

```
LoggerService.Initialize(new ReportingLoggerService(loggerFactory.CreateLogger("DXReporting")));
```

### Using custom exception handlers

Use services to pass message details to the client.
[Services/Reporting/CustomExceptionHandlers.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomExceptionHandlers.cs)

## IWebDocumentViewerAuthorizationService

To prevent user to access to documents, reports and exported documents, implement and register IWebDocumentViewerAuthorizationService along with WebDocumentViewerOperationLogger:

[Services/Reporting/DocumentViewerAuthorizationService.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/DocumentViewerAuthorizationService.cs)

Register it in the Startup as Services:

[Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs#L106)

## Skeleton

This will be updated in 20.1.5+.
I would recommend to show this approach for 20.1.5+
Move all scripts to the bottom of the [Views/Shared/\_Layout.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Shared/_Layout.cshtml)

```
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

And use two separate parts of the reporting control. RenderHtml() and RenderScripts() in a view:
for example [Views/Home/DesignReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml)

```
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
    @designerRender.GetScripts()
}
```

## Connection providers (serialize connection name only)

\*It does not matter prod or dev environment if you implement your own connection providers.

For report report designer, query builder and web document viewer.
Reporting services retrive the IConnectionProviderFactory and IDataSourceWizardConnectionStringsProvider from the DI container. Implementation of these services demonstrated in [Services/Reporting/CustomSqlDataConnectionProviderFactory.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomSqlDataConnectionProviderFactory.cs) and [Services/Reporting/CustomSqlDataSourceWizardConnectionStringsProvider.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Services/Reporting/CustomSqlDataSourceWizardConnectionStringsProvider.cs) accordingly.

To prevent passing to the client encrypted connection parameters for SqlDataSource instances, return null in the IDataSourceWizardConnectionStringsProvider .GetDataConnectionParameters method:

```
        public DataConnectionParametersBase GetDataConnectionParameters(string name) {
            return null;
        }
```

and resolve connection in the IConnectionProviderService, that return the CustomSqlDataConnectionProviderFactory.

```
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

and register these services in the [Startup.cs](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Startup.cs)

```
    services.AddScoped<IDataSourceWizardConnectionStringsProvider, CustomSqlDataSourceWizardConnectionStringsProvider>();
    services.AddScoped<IConnectionProviderService, CustomConnectionProviderService>();
    services.AddScoped<IConnectionProviderFactory, CustomSqlDataConnectionProviderFactory>();
```

## Localization (High)

First of all, FYI: we cannot load custom localization from [localization.devexpress.com](https://localization.devexpress.com/), localization services does not provide resources, assemblied for .net standard.
only json files are available for localizing the UI of the Reporting Controls.
So use [localization.devexpress.com](https://localization.devexpress.com/) to get you custom localized messages (json files), customize other devextreme messages (if needed), put it to the wwwroot folder, (to the inner folder `localization` that used in this example), after that configure a view [Views/Home/DesignReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DesignReport.cshtml) or [Views/Home/DisplayReport.cshtml](https://github.com/DevExpress-Examples/AspNetCore.Reporting.BestPractices/blob/master/AspNetCore.Reporting.BestPractices/Views/Home/DisplayReport.cshtml):

1. Load these json files on the `CustomizeLocalization` event from the client-side

```
    function CustomizeLocalization(s, e){
        e.LoadMessages($.get("/localization/reporting/dx-analytics-core.de.json"));
        e.LoadMessages($.get("/localization/reporting/dx-reporting.de.json"));
        e.LoadMessages($.get("/localization/devextreme/de.json").done(function(messages) { DevExpress.localization.loadMessages(messages); }));
        e.SetAvailableCultures(["de"]);
    }
```

2. Turn off automatic attaching the localization dictionary from server by the `IncludeLocalization` option:

```
    Html.DevExpress().ReportDesigner("ReportDesigner").
        .ClientSideModelSettings(clientSide => {
            clientSide.IncludeLocalization = false;
        })
        ...
```

## Ef Core as Report Data source

To Navigate/Link to the sample:
https://github.com/DevExpress-Examples/Reporting-Entity-Framework-Core-In-AspNet-Core
