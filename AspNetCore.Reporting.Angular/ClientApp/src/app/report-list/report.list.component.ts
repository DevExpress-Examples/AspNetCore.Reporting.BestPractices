import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'report-list-component',
  templateUrl: './report.list.component.html'
})
export class ReportListComponent {
  reportList: ReportItem[];
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<ReportItem[]>(baseUrl + 'reportlist').subscribe(result => {
      this.reportList = result;
    }, error => console.error(error));
  }
}

interface ReportItem {
  Id: string;
  Title: string;
}
