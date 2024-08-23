import { Routes } from '@angular/router';
import { AuthenticatedComponent } from './authenticated/authenticated.component';
import { LoginComponent } from './login/login.component';
import { authenticatedGuard } from './services/guards/authenticated.guard';

export const rootRouterConfig: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'my-budget',
  },

  {
    path: '',
    component: AuthenticatedComponent,
    canActivate: [authenticatedGuard],
    children: [
      {
        path: 'my-budget',
        loadChildren: () => import('./my-budget/my-budget.module').then((m) => m.MyBudgetModule),
      },
      {
        path: 'budgets',
        title: 'Budgets',
        loadChildren: () => import('./budgets/budgets.module').then((m) => m.BudgetsModule),
      },
      {
        path: 'requests',
        title: 'Requests',
        loadChildren: () => import('./requests/requests.module').then((m) => m.RequestsModule),
      },
      {
        path: 'employees',
        title: 'Employees',
        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
      },
      {
        path: 'statistics',
        title: 'Statistics',
        loadChildren: () => import('./statistics/statistics.module').then((m) => m.StatisticsModule),
      },
      {
        path: 'team-budget',
        title: 'Team Budget',
        loadChildren: () => import('./team-budget/team-budget.module').then((m) => m.TeamBudgetModule),
      },
    ],
  },
  { path: 'login', component: LoginComponent },
];
