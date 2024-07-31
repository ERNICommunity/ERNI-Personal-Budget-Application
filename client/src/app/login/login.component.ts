import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { lastValueFrom } from 'rxjs';
import { Button } from "primeng/button";

@Component({
  selector: 'pba-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [ Button ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  #msalService = inject(MsalService);
  #router = inject(Router);

  async login(): Promise<void> {
    await lastValueFrom(this.#msalService.loginPopup());
    this.#router.navigate(['my-budget']);
  }
}
