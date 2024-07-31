import { ChangeDetectionStrategy, Component, computed, Signal } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { MenuItem, MenuItemCommandEvent } from 'primeng/api';
import { AuthorizationPolicy, PolicyNames } from './services/authorization-policy';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthenticationService } from './services/authentication.service';
import { Router } from '@angular/router';
import { AuthenticatedGuard } from './services/guards/authenticated.guard';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  userMenuItems: MenuItem[] = [
    {
      label: 'Log out',
      icon: 'pi pi-sign-out',
      url: '#',
      command: (event: MenuItemCommandEvent) => {
        this.logout();
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
    {
      label: 'Conditions of use',
      icon: 'pi pi-file',
      url: 'https://erniegh.sharepoint.com/sites/people/benefit/ESK/Pages/Personal-budget.aspx',
      target: '_blank',
    },
  ];

  userInfo = toSignal(this.authService.userInfo$);
  isAuthenticated = toSignal(this.authenticatedGuard.isAuthenticated(), { initialValue: false });
  mainNavItems: Signal<(MenuItem & { policy?: PolicyNames })[]> = computed(() => {
    if (!this.userInfo() || !this.isAuthenticated()) {
      return [];
    }

    return this.#mainNavItems.map((item) => ({
      ...item,
      visible: !item.accessRight || AuthorizationPolicy.evaluate(item.accessRight, this.userInfo()),
    }));
  });

  constructor(
    private msal: MsalService,
    // !!! DO NOT REMOVE THIS: MsalBroadcastService has to be injected/created, the login won't work otherwise
    private msalBroadcast: MsalBroadcastService,
    private authService: AuthenticationService,
    private authenticatedGuard: AuthenticatedGuard,
    private router: Router,
  ) {
    this.msal.handleRedirectObservable().subscribe();
    //this.msal.instance.handleRedirectPromise();
  }

  async logout(): Promise<void> {
    await this.authService.logout();
    this.router.navigate(['/login']);
  }
}
