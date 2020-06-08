# ASP.NET Core Reporting - Best Practices

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
