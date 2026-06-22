import { Component } from '@angular/core';
import { LoginMenuComponent } from '../../api-authorization/login-menu/login-menu.component';
import { NgClass } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css'],
    standalone: true,
    imports: [RouterLink, NgClass, LoginMenuComponent, RouterLinkActive]
})
export class NavMenuComponent {
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
