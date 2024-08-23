import { ChangeDetectionStrategy, Component, computed, Signal } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { MenuItem, MenuItemCommandEvent } from 'primeng/api';
import { AuthorizationPolicy, PolicyNames } from './services/authorization-policy';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthenticationService } from './services/authentication.service';
import { filter, map, startWith, switchMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  isIframe = window !== window.parent && !window.opener;
  userMenuItems: MenuItem[] = [
    {
      label: 'Log out',
      icon: 'pi pi-sign-out',
      url: '#',
      command: (event: MenuItemCommandEvent) => {
        this.authService.logout();
        event.originalEvent?.preventDefault();
      },
    },
    {
      separator: true,
    },
  ];

  #mainNavItems: (MenuItem & { accessRight?: PolicyNames })[] = [
    {
      label: 'My Budget',
      icon: 'pi pi-wallet',
      accessRight: 'canAccessMyBudget',
      route: '/my-budget',
    },
    {
      label: 'Team Budget',
      accessRight: 'isSuperior',
      route: '/team-budget',
    },
    {
      label: 'Manage Budgets',
      accessRight: 'isAdmin',
      route: '/budgets',
    },
    {
      label: 'Requests',
      accessRight: 'canReadRequests',
      route: '/requests',
    },
    {
      label: 'Mass Request',
      accessRight: 'isAdmin',
      route: '/requests/mass-request',
    },
    {
      label: 'Employees',
      icon: 'pi pi-users',
      accessRight: 'isAdmin',
      route: '/employees',
    },
    {
      label: 'Statistics',
      icon: 'pi pi-chart-line',
      accessRight: 'isAdmin',
      route: '/statistics',
    },
  ];

  userInfo = toSignal(this.authService.userInfo$);
  isAuthenticated = toSignal(this.authService.isAuthenticated(), { initialValue: false });
  mainNavItems: Signal<(MenuItem & { policy?: PolicyNames })[]> = computed(() => {
    const userInfo = this.userInfo();
    if (!userInfo || !this.isAuthenticated()) {
      return [];
    }

    return this.#mainNavItems.map((item) => ({
      ...item,
      visible: !item.accessRight || AuthorizationPolicy.evaluate(item.accessRight, userInfo),
    }));
  });

  avatarUrl$ = this.authService.userInfo$.pipe(
    filter((userInfo) => !!userInfo),
    switchMap(() =>
      this.httpClient
        .get('https://graph.microsoft.com/v1.0/me/photos/48x48/$value', { responseType: 'blob' })
        .pipe(map((val) => URL.createObjectURL(val))),
    ),
    startWith(null),
  );

  constructor(
    private msal: MsalService,
    // !!! DO NOT REMOVE THIS: MsalBroadcastService has to be injected/created, the login won't work otherwise
    private msalBroadcast: MsalBroadcastService,
    private authService: AuthenticationService,
    private httpClient: HttpClient,
  ) {
    this.msal.handleRedirectObservable().subscribe();
    //this.msal.instance.handleRedirectPromise();
  }
}
