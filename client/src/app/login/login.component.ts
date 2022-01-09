import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserInfo } from '../model/userInfo';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'rmt-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  private subscription: Subscription;

  constructor(private readonly auth: AuthenticationService, private readonly router: Router) { }

  ngOnInit(): void {
    this.subscription = this.auth.userInfo$.subscribe(_ => this.handleUser(_));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  async login(): Promise<void> {
    this.auth.login();
  }

  private handleUser(userInfo: UserInfo | undefined): void {
    if (userInfo === undefined) {
      return;
    }

    this.router.navigate(['/my-budget']);
  }
}
