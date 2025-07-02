import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { DxReportViewerModule, DxReportDesignerModule } from 'devexpress-reporting-angular';
import { HomeComponent } from './home/home.component';
import { ReportListComponent } from './report-list/report.list.component';

import { ReportDesignerComponent } from './reportdesigner/report-designer';
import { ReportViewerComponent } from './reportviewer/report-viewer';
import { ApiAuthorizationModule } from '../api-authorization/api-authorization.module';
import { AuthorizeGuard } from '../api-authorization/authorize.guard';
import { AuthorizeInterceptor } from '../api-authorization/authorize.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ReportViewerComponent,
    ReportDesignerComponent,
    ReportListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    DxReportViewerModule,
    DxReportDesignerModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'designer', component: ReportDesignerComponent, canActivate: [AuthorizeGuard] },
      { path: 'viewer', component: ReportViewerComponent, canActivate: [AuthorizeGuard] },
      { path: 'report-list', component: ReportListComponent, canActivate: [AuthorizeGuard]},
    ])
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
