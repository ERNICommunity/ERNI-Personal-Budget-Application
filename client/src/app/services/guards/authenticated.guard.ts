import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { filter, Observable } from 'rxjs';
import { map, tap } from "rxjs/operators";

@Injectable({ providedIn: 'root' })
export class AuthenticatedGuard implements CanActivate {
  constructor(
    private msalService: MsalService,
    private router: Router,
    private msalBroadcastService: MsalBroadcastService
  ) {}

  isAuthenticated(): Observable<boolean> {
    return this.msalBroadcastService.inProgress$.pipe(
      filter((status: InteractionStatus) => status === InteractionStatus.None),
      map(() => this.msalService.instance.getAllAccounts().length > 0)
    );
  }

  canActivate(): Observable<boolean> | Promise<boolean> | boolean {
    return this.isAuthenticated().pipe(
      tap((isAuthenticated) => {
        if (!isAuthenticated) {
          this.router.navigate(['/login']);
        }
      })
    );
  }
}
