import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { DxReportViewerModule, DxReportDesignerModule } from 'devexpress-reporting-angular';
import { ReportViewerComponent } from './reportviewer/report-viewer';
import { ReportDesignerComponent } from './reportdesigner/report-designer';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
      HomeComponent,
    ReportViewerComponent,
    ReportDesignerComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
      FormsModule,
    DxReportViewerModule,
    DxReportDesignerModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'designer', component: ReportDesignerComponent },
      { path: 'viewer', component: ReportViewerComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
