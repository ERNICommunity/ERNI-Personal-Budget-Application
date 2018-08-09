import {Component, OnInit} from '@angular/core';
import {CategoryService} from '../../services/category.service';
import {Category} from "../../model/category";

@Component({
    selector: 'app-category-list',
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

    add(title: string): void {
        title = title.trim();
        if (!title) { return; }
        this.valueService.addCategory({ title } as Category)
            .subscribe(category => this.categories.push(category));
      }

      delete(category: Category): void {
        this.categories = this.categories.filter(cat => cat !== category);
        this.valueService.deleteCategory(category).subscribe();
      }
}
