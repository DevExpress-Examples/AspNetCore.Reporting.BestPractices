import { AppComponent } from './app/app.component';
import { ReportListComponent } from './app/report-list/report.list.component';
import { ReportViewerComponent } from './app/reportviewer/report-viewer';
import { AuthorizeGuard } from './api-authorization/authorize.guard';
import { ReportDesignerComponent } from './app/reportdesigner/report-designer';
import { HomeComponent } from './app/home/home.component';
import { provideRouter } from '@angular/router';
import { LoginComponent } from './api-authorization/login/login.component';
import { LogoutComponent } from './api-authorization/logout/logout.component';
import { ApplicationPaths } from './api-authorization/api-authorization.constants';
import { bootstrapApplication } from '@angular/platform-browser';
import { AuthorizeInterceptor } from './api-authorization/authorize.interceptor';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

bootstrapApplication(AppComponent, {
    providers: [
        { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
        { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
        provideHttpClient(withInterceptorsFromDi()),
        provideRouter([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'designer', component: ReportDesignerComponent, canActivate: [AuthorizeGuard] },
            { path: 'viewer', component: ReportViewerComponent, canActivate: [AuthorizeGuard] },
            { path: 'report-list', component: ReportListComponent, canActivate: [AuthorizeGuard] },
            { path: ApplicationPaths.Login, component: LoginComponent },
            { path: ApplicationPaths.LoginFailed, component: LoginComponent },
            { path: ApplicationPaths.LoginCallback, component: LoginComponent },
            { path: ApplicationPaths.Register, component: LoginComponent },
            { path: ApplicationPaths.Profile, component: LoginComponent },
            { path: ApplicationPaths.LogOut, component: LogoutComponent },
            { path: ApplicationPaths.LoggedOut, component: LogoutComponent },
            { path: ApplicationPaths.LogOutCallback, component: LogoutComponent },
        ])
    ]
})
  .catch(err => console.log(err));
