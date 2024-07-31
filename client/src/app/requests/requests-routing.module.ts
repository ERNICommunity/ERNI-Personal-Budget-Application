import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RequestsComponent } from './requests.component';

const requestRoutes: Routes = [
  {
    path: 'requests',
    component: RequestsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(requestRoutes)],
  exports: [RouterModule],
})
export class RequestsRoutingModule {}
