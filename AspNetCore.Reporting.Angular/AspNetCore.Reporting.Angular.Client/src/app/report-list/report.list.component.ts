import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgFor } from '@angular/common';

@Component({
    selector: 'report-list-component',
    templateUrl: './report.list.component.html',
    standalone: true,
    imports: [NgFor, RouterLink, RouterLinkActive]
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
