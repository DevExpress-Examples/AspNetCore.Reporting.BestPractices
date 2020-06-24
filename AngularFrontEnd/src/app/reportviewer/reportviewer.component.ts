import { Component, Inject, ViewEncapsulation, OnInit } from '@angular/core';
import DevExpress from "devexpress-reporting";

@Component({
  selector: 'app-reportviewer',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './reportviewer.component.html',
  styleUrls: [
    "../../../node_modules/jquery-ui/themes/base/all.css",
    "../../../node_modules/devextreme/dist/css/dx.common.css",
    "../../../node_modules/devextreme/dist/css/dx.light.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
    "../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css",
    "./reportviewer.component.less"
  ]
})
export class ReportViewerComponent implements OnInit {
  reportUrl: string = "TestReport";
  invokeAction: string = '/DXXRDV';
  host: string = "http://localhost:62876/";

  constructor(@Inject('BASE_URL') public hostUrl: string) { }

  ngOnInit(): void {
    DevExpress.Reporting.Viewer.Settings.AsyncExportApproach = true;
  }
}