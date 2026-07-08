import { fetchSetup } from "@devexpress/analytics-core/analytics-utils"
import { Component, ViewEncapsulation, OnInit, signal } from '@angular/core';
import { environment } from "../../environments/environment";
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { ActivatedRoute } from '@angular/router';
import { DxReportDesignerModule, DxReportViewerModule } from "devexpress-reporting-angular";

@Component({
    selector: 'report-designer',
    encapsulation: ViewEncapsulation.None,
    templateUrl: './report-designer.html',
    styleUrls: [
        "../../../node_modules/ace-builds/css/ace.css",
        "../../../node_modules/ace-builds/css/theme/dreamweaver.css",
        "../../../node_modules/devextreme/dist/css/dx.material.blue.light.css",
        "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
        "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.material.blue.light.css",
        "../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css",
        "../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css",
        "../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css"
    ],
    imports: [DxReportDesignerModule, DxReportViewerModule]
})

export class ReportDesignerComponent implements OnInit {
  getDesignerModelAction = "api/ReportDesignerSetup/GetReportDesignerModel";
  hostUrl = environment.serverUri;
  reportUrl = signal('');

  constructor(private authorize: AuthorizeService, private activateRoute: ActivatedRoute) {
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
      this.reportUrl.set(this.activateRoute.snapshot.queryParams['reportId']);
    }
  }
}
