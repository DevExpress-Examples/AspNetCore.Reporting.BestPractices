import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../authorize.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';

@Component({
    selector: 'app-login-menu',
    templateUrl: './login-menu.component.html',
    styleUrls: ['./login-menu.component.css'],
    imports: [RouterLink, AsyncPipe]
})
export class LoginMenuComponent implements OnInit {
  public isAuthenticated!: Observable<boolean>;
  public userName!: Observable<string | null | undefined>;

  constructor(private authorizeService: AuthorizeService) { }

  ngOnInit() {
    this.isAuthenticated = this.authorizeService.isAuthenticated();
    this.userName = this.authorizeService.getUser().pipe(map(u => u && u.name));
  }
}
