import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginMenuComponent } from './login-menu/login-menu.component';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';
import { RouterModule } from '@angular/router';
import { ApplicationPaths } from './api-authorization.constants';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

@NgModule({ declarations: [LoginMenuComponent, LoginComponent, LogoutComponent],
    exports: [LoginMenuComponent, LoginComponent, LogoutComponent], imports: [CommonModule,
        RouterModule.forChild([
            { path: ApplicationPaths.Register, component: LoginComponent },
            { path: ApplicationPaths.Profile, component: LoginComponent },
            { path: ApplicationPaths.Login, component: LoginComponent },
            { path: ApplicationPaths.LoginFailed, component: LoginComponent },
            { path: ApplicationPaths.LoginCallback, component: LoginComponent },
            { path: ApplicationPaths.LogOut, component: LogoutComponent },
            { path: ApplicationPaths.LoggedOut, component: LogoutComponent },
            { path: ApplicationPaths.LogOutCallback, component: LogoutComponent }
        ])], providers: [provideHttpClient(withInterceptorsFromDi())] })
export class ApiAuthorizationModule { }
