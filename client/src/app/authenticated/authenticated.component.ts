import { ChangeDetectionStrategy, Component, Signal } from "@angular/core";
import { Router } from "@angular/router";
import { AuthenticationService } from "../services/authentication.service";
import { SharedModule } from "../shared/shared.module";
import { MenubarModule } from "primeng/menubar";
import { MenuItem } from "primeng/api";
import { Ripple } from "primeng/ripple";
import { AuthorizationPolicy, PolicyNames } from "../services/authorization-policy";
import { filter, map } from "rxjs/operators";
import { toSignal } from "@angular/core/rxjs-interop";
import { NgOptimizedImage } from "@angular/common";

@Component({
  selector: "authenticated",
  templateUrl: "./authenticated.component.html",
  styleUrls: ["./authenticated.component.scss"],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [SharedModule, MenubarModule, Ripple, NgOptimizedImage],
})
export class AuthenticatedComponent {

  #items: (MenuItem & { accessRight?: PolicyNames })[] = [
    {
      label: 'My Budget',
      accessRight: 'canAccessMyBudget',
      route: "/my-budget",
    },
    {
      label: 'Team Budget',
      accessRight: 'isSuperior',
      route: "/team-budget"
    },
    {
      label: 'Manage Budgets',
      accessRight: 'isAdmin',
      route: "/budgets"
    },
    {
      label: 'Requests',
      accessRight: 'canReadRequests',
      route: "/requests"
    },
    {
      label: 'Mass Request',
      accessRight: 'isAdmin',
      route: "/requests/mass-request"
    },
    {
      label: 'Employees',
      icon: 'pi pi-users',
      accessRight: 'isAdmin',
      route: "/employees"
    },
    {
      label: 'Statistics',
      icon: 'pi pi-chart-line',
      accessRight: 'isAdmin',
      route: "/statistics"
    },
    {
      label: 'Conditions of use',
      icon: 'pi pi-file',
      url: 'https://erniegh.sharepoint.com/sites/people/benefit/ESK/Pages/Personal-budget.aspx',
      target: '_blank'
    },
  ];

  public items: Signal<(MenuItem & { policy?: PolicyNames })[]> = toSignal(
    this.authService.userInfo$.pipe(
      filter(userInfo => !!userInfo),
      map((userInfo) => this.#items.map(item => {
        return {
          ...item,
          visible: !item.accessRight ||  AuthorizationPolicy.evaluate(item.accessRight, userInfo)
        };
      })),
    ),
    { initialValue: [] }
  );

  constructor(
    public authService: AuthenticationService,
    public router: Router
  ) {}

  async logout(): Promise<void> {
    await this.authService.logout();
    this.router.navigate(["/login"]);
  }
}
