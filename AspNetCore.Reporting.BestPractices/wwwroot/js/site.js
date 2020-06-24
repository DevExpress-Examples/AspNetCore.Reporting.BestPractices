function SetupJwt(bearerToken, xsrf) {
    DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = {
        headers: {
            //'Authorization': 'Bearer ' + bearerToken,
            'RequestVerificationToken': xsrf
        }
    }; 
}
function ViewerInit(s, e) {
    DevExpress.Reporting.Viewer.Settings.AsyncExportApproach = true;
}
