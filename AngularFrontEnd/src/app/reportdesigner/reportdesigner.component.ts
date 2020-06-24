import { Component, Inject, ViewEncapsulation, Injectable, OnInit } from '@angular/core';
import 'devexpress-reporting/dx-richedit';
import dxa from "@devexpress/analytics-core";
import DevExpress from "devexpress-reporting";
import * as ko from "knockout";
import { AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-reportdesigner',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './reportdesigner.component.html',
  styleUrls: [
    "../../../node_modules/jquery-ui/themes/base/all.css",
    "../../../node_modules/devextreme/dist/css/dx.common.css",
    "../../../node_modules/devextreme/dist/css/dx.light.css",
    "../../../node_modules/devexpress-richedit/dist/dx.richedit.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
    "../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css",
    "../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css",
    "../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css",
    "./reportdesigner.component.less"
  ]
})
@Injectable()
export class ReportDesignerComponent implements OnInit {
  host: string = "http://localhost:62876/";
  getDesignerModelAction = "api/ReportDesigner/GetReportDesignerModel";
  reportUrl = "CourseListReport";

    constructor(@Inject('BASE_URL') public hostUrl: string, private authenticationService: AuthenticationService) { }

    ngOnInit(): void {
      var currentUser = this.authenticationService.currentUserValue;
      if( currentUser && currentUser.token)
      dxa.Analytics.Utils.ajaxSetup.ajaxSettings = {  
        headers: { 'Authorization': "Bearer " + currentUser.token }  
      };
    }

    BeforeRender(event) {
    }
}
