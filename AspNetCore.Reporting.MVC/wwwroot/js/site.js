function SetupJwt(bearerToken, xsrf) {
    DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = {
        headers: {
            //'Authorization': 'Bearer ' + bearerToken,
            'RequestVerificationToken': xsrf
        }
    }; 
}

function AttachXSRFToken_OnExport(args, xsrf) {
    args.FormData["__RequestVerificationToken"] = xsrf;
}