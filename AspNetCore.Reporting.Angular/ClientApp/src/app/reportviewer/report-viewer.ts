import { Component, Inject, ViewEncapsulation } from '@angular/core';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import DevExpress from "@devexpress/analytics-core";

@Component({
  selector: 'report-viewer',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './report-viewer.html',
  styleUrls: [
    "../../../node_modules/jquery-ui/themes/base/all.css",
    "../../../node_modules/devextreme/dist/css/dx.common.css",
    "../../../node_modules/devextreme/dist/css/dx.light.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
    "../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css"
  ]
})
export class ReportViewerComponent {
  reportUrl: string = "TestReport";
  invokeAction: string = '/DXXRDVAngular';

  constructor(@Inject('BASE_URL') public hostUrl: string, private authorize: AuthorizeService) {
    this.authorize.getAccessToken()
      .subscribe(x => {
        DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = {
          headers: {
            'Authorization': 'Bearer ' + x
          }
        };
      });
  }
}
