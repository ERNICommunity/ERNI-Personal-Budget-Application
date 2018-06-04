import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {UsersComponent} from "./users.component";

const userRoutes: Routes = [
    {
        path: 'users',
        component: UsersComponent,
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(userRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class UsersRoutingModule {
}