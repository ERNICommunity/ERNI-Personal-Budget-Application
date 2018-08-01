import {Component, OnInit} from '@angular/core';
import {CategoryService} from '../../services/category.service';
import {Category} from "../../model/category";

@Component({
    selector: 'app-users',
    templateUrl: './CategoryList.component.html',
    styleUrls: ['./CategoryList.component.css']
})
export class CategoryListComponent implements OnInit {
    categories: Category[];

    constructor(private valueService: CategoryService) {
    }

    ngOnInit() {
        this.getCategories();
    }

    getCategories(): void {
        this.valueService.getCategories()
            .subscribe(categories => this.categories = categories);
    }

}
