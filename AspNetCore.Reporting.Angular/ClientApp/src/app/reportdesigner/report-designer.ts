import { Component, Inject, ViewEncapsulation, OnInit } from '@angular/core';
import DevExpress from "@devexpress/analytics-core";
import DevExpressReporting from "devexpress-reporting";
import { AuthorizeService } from '../../api-authorization/authorize.service';
import * as ko from 'knockout';
import { ActivatedRoute } from '@angular/router';

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

export class ReportDesignerComponent implements OnInit {
  getDesignerModelAction = "api/ReportDesignerSetup/GetReportDesignerModel";
  get reportUrl() {
    return this.koReportUrl();
  };
  set reportUrl(newUrl) {
    this.koReportUrl(newUrl);
  }
  koReportUrl = ko.observable('');

  constructor(@Inject('BASE_URL') public hostUrl: string, private authorize: AuthorizeService, private activateRoute: ActivatedRoute) {
    this.authorize.getAccessToken()
      .subscribe(x => {
        DevExpress.Analytics.Utils.ajaxSetup.ajaxSettings = {
          headers: {
            'Authorization': 'Bearer ' + x
          }
        };
      });
    DevExpressReporting.Reporting.Viewer.Settings.AsyncExportApproach = true;
  }

  ngOnInit() {
    if(this.activateRoute.snapshot.queryParams['reportId']) {
      this.reportUrl = this.activateRoute.snapshot.queryParams['reportId'];
    }
  }
}
