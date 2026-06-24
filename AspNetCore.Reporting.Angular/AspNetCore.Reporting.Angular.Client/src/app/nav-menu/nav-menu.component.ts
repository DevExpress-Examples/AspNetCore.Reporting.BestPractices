import { Component, signal } from '@angular/core';
import { LoginMenuComponent } from '../../api-authorization/login-menu/login-menu.component';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css'],
    imports: [RouterLink, LoginMenuComponent, RouterLinkActive]
})
export class NavMenuComponent {
  protected readonly isExpanded = signal(false);

  collapse() {
    this.isExpanded.set(false);
  }

  toggle() {
    this.isExpanded.update(value => !value);
  }
}
