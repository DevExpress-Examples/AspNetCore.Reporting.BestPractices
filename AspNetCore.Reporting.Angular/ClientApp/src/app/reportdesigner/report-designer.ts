import { Component, Inject, ViewEncapsulation } from '@angular/core';
import DevExpress from "@devexpress/analytics-core";
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Component({
  selector: 'report-designer',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './report-designer.html',
  styleUrls: [
    "../../../node_modules/jquery-ui/themes/base/all.css",
    "../../../node_modules/devextreme/dist/css/dx.common.css",
    "../../../node_modules/devextreme/dist/css/dx.light.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css",
    "../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css",
    "../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css"
  ]
})

export class ReportDesignerComponent {
  getDesignerModelAction = "api/ReportDesignerSetup/GetReportDesignerModel";
  reportUrl = "TestReport";

  constructor(@Inject('BASE_URL') public hostUrl: string, private authorize: AuthorizeService) {
    this.authorize.getAccessToken()
      .subscribe(x => {
        DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = {
          headers: {
            'Authorization': 'Bearer ' + x
          }
        };
      });}
}
