import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { fetchSetup } from '@devexpress/analytics-core/analytics-utils';
import * as ko from 'knockout';
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Component({
  selector: 'report-viewer',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './report-viewer.html',
  styleUrls: [
    "../../../node_modules/devextreme/dist/css/dx.material.blue.light.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.material.blue.light.css",
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

  useSameTabExport = true;
  useAsynchronousExport = true;

  constructor(@Inject('BASE_URL') public hostUrl: string, private authorize: AuthorizeService, private activateRoute: ActivatedRoute) {
    this.authorize.getAccessToken()
      .subscribe(x => {
        fetchSetup.fetchSettings = {
          headers: {
            'Authorization': 'Bearer ' + x
          }
        };
      });
  }

  ngOnInit() {
    if(this.activateRoute.snapshot.queryParams['reportId']) {
      this.reportUrl = this.activateRoute.snapshot.queryParams['reportId'];
    }
  }
}
