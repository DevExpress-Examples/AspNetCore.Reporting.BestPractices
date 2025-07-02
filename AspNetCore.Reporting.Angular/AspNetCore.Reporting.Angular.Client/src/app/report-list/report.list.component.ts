import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'report-list-component',
  templateUrl: './report.list.component.html'
})
export class ReportListComponent {
  reportList?: ReportItem[];
  constructor(http: HttpClient) {
    http.get<ReportItem[]>(environment.serverUri + 'reportlist').subscribe(result => {
      this.reportList = result;
    }, error => console.error(error));
  }
}

interface ReportItem {
  id: string;
  title: string;
}
