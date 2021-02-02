import { AsyncExportApproach } from "devexpress-reporting/scopes/reporting-viewer-settings"
import { ajaxSetup } from "@devexpress/analytics-core/analytics-utils"
import { Component, Inject, ViewEncapsulation, OnInit } from '@angular/core';
import { AuthorizeService } from '../../api-authorization/authorize.service';


import { ActivatedRoute } from '@angular/router';
import * as ko from 'knockout';

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
export class ReportViewerComponent implements OnInit {
  get reportUrl() {
    return this.koReportUrl();
  };
  set reportUrl(newUrl) {
    this.koReportUrl(newUrl);
  }
  koReportUrl = ko.observable('');
  invokeAction: string = '/DXXRDVAngular';


  constructor(@Inject('BASE_URL') public hostUrl: string, private authorize: AuthorizeService, private activateRoute: ActivatedRoute) {
    this.authorize.getAccessToken()
      .subscribe(x => {
        ajaxSetup.ajaxSettings = {
          headers: {
            'Authorization': 'Bearer ' + x
          }
        };
      });

    AsyncExportApproach(true);
  }

  ngOnInit() {
    if(this.activateRoute.snapshot.queryParams['reportId']) {
      this.reportUrl = this.activateRoute.snapshot.queryParams['reportId'];
    }
  }
}
