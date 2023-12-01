import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { AdminRoleGuard } from '../services/guards/admin-role.guard';
import { StatisticsComponent } from './statistics/statistics.component';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        RouterModule.forChild([
            {
                path: '',
                canActivate: [AdminRoleGuard],
                children: [
                    {
                        path: '',
                        redirectTo: new Date().getFullYear().toString(),
                        pathMatch: 'full'
                    },
                    {
                        path: ':year',
                        component: StatisticsComponent,
                        canActivate: [AdminRoleGuard]
                    }
                ]
            }
        ]),
        StatisticsComponent
    ]
})
export class StatisticsModule {}
