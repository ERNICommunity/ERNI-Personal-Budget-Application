import {Component, OnInit} from '@angular/core';
import {Category} from '../model/category';
import {CategoryService} from '../services/category.service';

@Component({
    selector: 'app-categories',
    templateUrl: './categories.component.html',
    styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
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
