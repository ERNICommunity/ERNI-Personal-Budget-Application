import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {CategoriesComponent} from "./categories.component";

const categoriesRoutes: Routes = [
    {
        path: 'categories',
        component: CategoriesComponent,
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(categoriesRoutes)
    ],
    exports: [
        RouterModule
    ]
})
export class CategoriesRoutingModule {
}