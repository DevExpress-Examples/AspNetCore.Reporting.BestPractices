function SetupJwt(bearerToken, xsrf) {
    DevExpress.Analytics.Utils.fetchSetup.fetchSettings = {
        headers: {
            //'Authorization': 'Bearer ' + bearerToken,
            'RequestVerificationToken': xsrf
        }
    }; 
}