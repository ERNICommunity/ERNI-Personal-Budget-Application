import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { adminGuard } from '../services/guards/admin.guard';
import { StatisticsComponent } from './statistics/statistics.component';

@NgModule({
  declarations: [StatisticsComponent],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([
      {
        path: '',
        canActivate: [adminGuard],
        children: [
          {
            path: '',
            redirectTo: new Date().getFullYear().toString(),
            pathMatch: 'full',
          },
          {
            path: ':year',
            component: StatisticsComponent,
          },
        ],
      },
    ]),
  ],
})
export class StatisticsModule {}
